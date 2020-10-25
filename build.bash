#!/usr/bin/env bash

# Stop script if unbound variable found (use ${var:-} if intentional)
# set -u
# By default cmd1 | cmd2 returns exit code of cmd2 regardless of cmd1 success
# This is causing it to fail
set -o pipefail

exec 3>&1

dotnet_exe="dotnet"
npm_exe="npm"

ScriptRoot="$( cd "$( dirname "${BASH_SOURCE[0]}" )" >/dev/null 2>&1 && pwd )"

tool_path="$ScriptRoot/tools"
node_install_path="$tool_path/node"
dotnet_version="3.1.403"
node_version="12.19.0"
node_version="v$node_version"
node_distro="win-x64"

dotnet_install_path="$tool_path/dotnet/dotnet-sdk-$dotnet_version"


get_current_os_name() {
    local uname=$(uname)
    if [ "$uname" = "Darwin" ]; then
        echo "darwin"
        return 0
    elif [ "$uname" = "Linux" ]; then
        echo "linux"
        return 0
    fi

    return 1
}

osname="$(get_current_os_name)" || return 1

if [ "$osname" = "darwin" ]; then
    node_distro="darwin-x64"
elif [ "$osname" = "linux" ]; then
    node_distro="linux-x64"
fi

node_dist="node-$node_version-$node_distro"
node_dist_path="$node_install_path/$node_dist"

# dotnet_exist=true
local_dotnet_exist=false
# npm_exist=true
local_npm_exist=false

if [ -d "$dotnet_install_path" ]; then
    echo "发现本地安装的 .NET Core: $dotnet_install_path"
    local_dotnet_exist=true
    export PATH="$dotnet_install_path":"$PATH"
    export DOTNET_ROOT="$dotnet_install_path"
fi

if [ -d "$node_dist_path" ]; then
    echo "发现本地安装的 Nodejs: $node_dist_path"
    local_npm_exist=true
    node_dist_path="$node_dist_path/bin"
    export PATH="$node_dist_path":"$PATH"
fi


# command -v $dotnet_exe >/dev/null 2>&1 || { dotnet_exist=false; }
# command -v $npm_exe >/dev/null 2>&1 || { npm_exist=false; }

# if [ "$local_dotnet_exist" = false ]; then
#     if [ "$dotnet_exist" = true ]; then
#         echo "使用 Path 的 .NET Core"
#     fi
# fi

# if [ "$local_npm_exist" = false ]; then
#     if [ "$dotnet_exist" = true ]; then
#         echo "使用 Path 的 NodeJs"
#     fi
# fi

if [ "$local_dotnet_exist" = false ]; then
    echo "未发现本地 .NET Core, 将进行安装"
    # 安装 dotnet
    dotnet_install_url="https://dot.net/v1/dotnet-install.sh"
    dotnet_install_file="$tool_path/dotnet-install.sh"

    if [ ! -d "$dotnet_install_path" ]; then
        mkdir -p $dotnet_install_path
    fi

    echo "正在下载 .NET Core 安装脚本"
    curl -s -L $dotnet_install_url -o $dotnet_install_file  >/dev/null

    echo "正在安装 .NET Core $dotnet_version"
    chmod +x $dotnet_install_file
    bash $dotnet_install_file --channel Current --version $dotnet_version --install-dir $dotnet_install_path --no-path

    export PATH="$dotnet_install_path":"$PATH"
    export DOTNET_ROOT="$dotnet_install_path"
fi

useMagic=false
CAKE_ARGUMENTS=()

# Parse arguments.
for i in "$@"; do
    case $1 in
        -s|--script) SCRIPT="$2"; shift ;;
        -useMagic) useMagic=true ;;
        --) shift; CAKE_ARGUMENTS+=("$@"); break ;;
        *) CAKE_ARGUMENTS+=("$1") ;;
    esac
    shift
done

if [ "$local_npm_exist" = false ]; then
    echo "未发现本地 NodeJs, 将进行安装"
    node_ext="tar.xz"
    node_arc="$node_dist.$node_ext"
    node_downloaded_file="$tool_path/$node_arc"
    official_node_dist="https://nodejs.org/dist/"
    taobao_node_dist="https://npm.taobao.org/mirrors/node/"
    node_url=""

    if [ "$useMagic" = true ]; then
        node_url="$taobao_node_dist$node_version/$node_arc"
    else
        node_url="$official_node_dist$node_version/$node_arc"
    fi

    echo "正在下载 $node_url"
    curl -s -L $node_url -o $node_downloaded_file >/dev/null

    if [ ! -d "$node_install_path" ]; then
        mkdir -p $node_install_path
    fi

    echo "正在解压 $node_arc"
    tar -xJf $node_downloaded_file -C $node_install_path >/dev/null

    node_dist_path="$node_dist_path/bin"
    export PATH="$node_dist_path":"$PATH"

    rm -f $node_downloaded_file
fi

cake_file="$tool_path/dotnet-cake"

echo "正在安装 Cake"
$dotnet_exe tool install --tool-path $tool_path Cake.Tool >/dev/null 2>&1

exec "$cake_file" "-useMagic=$useMagic" "${CAKE_ARGUMENTS[@]}"