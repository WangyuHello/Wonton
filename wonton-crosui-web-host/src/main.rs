use std::env;
use actix_files as fs;
use actix_web::{App, HttpServer};

mod coreclr;

#[actix_rt::main]
async fn main() -> std::io::Result<()> {
    let mut found = false;
    let mut webport_str: String = String::new();
    for argument in env::args() {
        if argument.to_uppercase().contains("ELECTRONWEBPORT") {
            found = true;
            webport_str = argument;
            break;
        }
    }

    let mut webport = 55405;

    if found {
        let webport_t = webport_str.replace("/ELECTRONWEBPORT=", "");
        webport = webport_t.parse().expect("Error parse web port");
    }

    println!("ELECTRONASPNETCORESTAERTED");

    HttpServer::new(|| {
        App::new().service(fs::Files::new("/", "./dist").index_file("index.html"))
    })
    .bind(format!("127.0.0.1:{}", webport))?
    .run()
    .await
}
