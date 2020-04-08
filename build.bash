#!/usr/bin/env bash

# Stop script on NZEC
set -e
# Stop script if unbound variable found (use ${var:-} if intentional)
set -u
# By default cmd1 | cmd2 returns exit code of cmd2 regardless of cmd1 success
# This is causing it to fail
set -o pipefail

exec 3>&1

dotnet_exe="dotnet"
npm_exe="npm"

ScriptRoot="$( cd "$( dirname "${BASH_SOURCE[0]}" )" >/dev/null 2>&1 && pwd )"

tool_path="$ScriptRoot/tools"
dotnet_install_path="$tool_path/dotnet"
node_install_path="$tool_path/node"
VERSION="v13.12.0"
DISTRO="win-x64"

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
    DISTRO="darwin-x64"
elif [ "$osname" = "linux" ]; then
    DISTRO="linux-x64"
fi

node_dist="node-$VERSION-$DISTRO"
node_dist_path="$node_install_path/$node_dist"

dotnet_exist=true
local_dotnet_exist=false
npm_exist=true
local_npm_exist=false

if [ -d "$dotnet_install_path" ]; then
    echo "发现本地安装的 .NET Core: $dotnet_install_path"
    local_dotnet_exist=true
    export PATH="$dotnet_install_path":"$PATH"
    export DOTNET_ROOT="$dotnet_install_path"
fi

if [ -d "$node_install_path" ]; then
    node_dist_path="$node_install_path/$node_dist"

    if [ -d "$node_dist_path" ]; then
        echo "发现本地安装的 Nodejs: $node_dist_path"
        local_npm_exist=true
        node_dist_path="$node_dist_path/bin"
        export PATH="$node_dist_path":"$PATH"
    fi
fi

command -v $dotnet_exe >/dev/null 2>&1 || { dotnet_exist=false; }
command -v $npm_exe >/dev/null 2>&1 || { npm_exist=false; }

if [ "$local_dotnet_exist" = false ]; then
    if [ "$dotnet_exist" = true ]; then
        echo "使用 Path 的 .NET Core"
    fi
fi

if [ "$local_npm_exist" = false ]; then
    if [ "$dotnet_exist" = true ]; then
        echo "使用 Path 的 NodeJs"
    fi
    
fi

if [ "$dotnet_exist" = false ]; then
    echo "未发现 .NET Core, 将进行安装"
    # 安装 dotnet
    dotnet_install_url="https://dot.net/v1/dotnet-install.sh"
    dotnet_install_file="$tool_path/dotnet-install.sh"

    if [ ! -d "$tool_path" ]; then
        mkdir $tool_path
    fi

    if [ ! -d "$dotnet_install_path" ]; then
        mkdir $dotnet_install_path
    fi

    echo "正在下载 .NET Core 安装脚本"
    wget -O $dotnet_install_file $dotnet_install_url

    echo "正在安装 .NET Core"
    chmod +x $dotnet_install_file
    bash $dotnet_install_file --channel Current --version Latest --install-dir $dotnet_install_path --no-path

    export PATH="$dotnet_install_path":"$PATH"
    export DOTNET_ROOT="$dotnet_install_path"
fi

useMagic=false

while [ $# -ne 0 ]
do
    name="$1"
    case "$name" in
        -useMagic)
            useMagic=true
            ;;
    esac

    shift
done

if [ "$npm_exist" = false ]; then
    echo "未发现 NodeJs, 将进行安装"
    node_ext="tar.xz"
    node_arc="$node_dist.$node_ext"
    node_downloaded_file="$tool_path/$node_arc"
    official_node_dist="https://nodejs.org/dist/"
    tuna_node_dist="https://mirrors.tuna.tsinghua.edu.cn/nodejs-release/"
    node_url=""

    if [ "$useMagic" = true ]; then
        node_url="$tuna_node_dist$VERSION/$node_arc"
    else
        node_url="$official_node_dist$VERSION/$node_arc"
    fi

    echo "正在下载 $node_url"
    wget -O $node_downloaded_file $node_url

    if [ ! -d "$node_install_path" ]; then
        mkdir $node_install_path
    fi

    echo "正在解压 $node_arc"
    tar -xJvf $node_downloaded_file -C $node_install_path

    node_dist_path="$node_dist_path/bin"
    export PATH="$node_dist_path":"$PATH"
fi

cake_file="$tool_path/dotnet-cake"

if  [ ! -f "$cake_file" ]; then
    echo "未发现 Cake, 将进行安装"
    dotnet tool install --tool-path $tool_path Cake.Tool 2>&1 || { echo ""; }
else
    echo "发现本地安装的 Cake: $cake_file"
fi

$cake_file -useMagic=$useMagic