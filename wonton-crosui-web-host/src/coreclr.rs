extern crate libloading as lib;

const coreclr_initialize: &[u8] = b"coreclr_initialize";
const coreclr_shutdown: &[u8] = b"coreclr_shutdown";
const coreclr_shutdown_2: &[u8] = b"coreclr_shutdown_2";
const coreclr_create_delegate: &[u8] = b"coreclr_create_delegate";
const coreclr_execute_assembly: &[u8] = b"coreclr_execute_assembly";

const coreclr_win: &str = "coreclr.dll";
const coreclr_unix: &str = "libcoreclr.so";

pub struct CoreCLR {
    coreclr_library: lib::Library
}

#[cfg(target_os = "windows")]
fn get_coreclr_lib_name() -> &'static str {
    coreclr_win
}

#[cfg(not(target_os = "windows"))]
fn get_coreclr_lib_name() -> &'static str {
    coreclr_unix
}

impl CoreCLR {
    pub fn new() -> Result<Self, Box<dyn std::error::Error>> {
        let lib_name = get_coreclr_lib_name();
        let lib = lib::Library::new(format!("commom/{}", lib_name))?;
        Ok(CoreCLR {
            coreclr_library: lib
        })
    }

    pub unsafe fn initialize(&self) -> Result<(u32), Box<dyn std::error::Error>> {
        let func: lib::Symbol<unsafe extern fn() -> u32> = self.coreclr_library.get(coreclr_initialize)?;
        Ok(func())
    }
}