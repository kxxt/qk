(*
    A simple utility made by kxxt (https://github.com/kxxt)
    |> while learning F#
*)
open System.Diagnostics


let dataDrive = "D:\\"

let shellExecute str = 
    ProcessStartInfo(FileName=str,UseShellExecute=true) |> Process.Start
type Address = 
    | LocalFS of string
    | URL of string
    | App of string
    member this.execute() =
        match this with
        | App str -> shellExecute str
        | URL str -> shellExecute str
        | LocalFS str -> 
            ProcessStartInfo(
                FileName="explorer.exe", 
                Arguments = str,
                UseShellExecute=true
            ) |> Process.Start
        |> ignore

let changeDirectory dir =
    printfn "cd into %s" dir

type Command = 
    | Go of Address option
    | Launch of string option
    | Cd of string option
    member this.execute() =
        match this with
        | Go(Some addr) -> addr.execute()
        | Launch(Some str) -> shellExecute str |> ignore
        | Cd(Some dir) -> changeDirectory(dir)
        | _ -> printfn "%s" "We can't understand your command."

let parseLocationArgument strs = 
    match strs with
    | head::tail -> 
        match head with
        | "repo" -> 
            let repos = dataDrive + "Repos\\"
            match tail with
            | sub::_ -> 
                match sub with
                | "i" -> Some(repos + "Mine\\Personal\\Current\\Independent\\")
                | "exp" | "e" -> Some(repos + "Mine\\Experimental")
                | "l" -> Some(repos + "Learn\\")
                | "c" -> Some(repos + "Cloned\\")
                | _ -> None
            | [] -> Some(repos)
        | "work" -> 
            match tail with
            | sub::_ ->
                match sub with
                | _ -> Some(dataDrive + "BigHomework\\")
            | [] -> Some(dataDrive + "BigHomework\\")
        | _ -> None          
    | [] -> None

let parseLaunchArgument strs = 
    None

let parseGoArgument strs = 
    match parseLocationArgument strs with
    | Some path -> Some(LocalFS path)
    | None -> 
        match strs with
        | head::tail ->
            match head with
            | "settings" | "setting" ->
                let setting = "ms-settings:"
                match tail with
                | sub::_ -> 
                    match sub with
                    | "disp" | "display" -> App(setting + "display")
                    | "net" | "network" | "internet" -> App(setting + "network")
                    | "update" | "upgrade" -> App(setting + "windowsupdate")
                    | _ -> App(setting + sub)
                    |> Some
                | [] -> Some(App setting)
            | _ -> None
        | [] -> None

let (|ParseCommand|_|) strs = 
    match strs with
    | head::tail ->
        match head with 
        | "go" | "goto" -> Some(Go(parseGoArgument(tail)))
        | "cd" -> Some(Cd(parseLocationArgument(tail)))
        | "launch" | "app" | "start" ->
            Some(Launch(parseLaunchArgument(tail)))
        | _ -> None
    | [] -> None


[<EntryPoint>]
let main argv =
    match argv |> Seq.toList with
    | ParseCommand command -> command.execute()
    | head::_ -> printfn "Invalid command: %s" head
    | [] -> printfn """qk: A simple quick tool written in F#.
Available commands: go(goto), cd, launch(app/start)        
"""
    0 // return an integer exit code