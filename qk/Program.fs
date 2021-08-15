(*
    A simple utility made by kxxt (https://github.com/kxxt/qk)
    |> while learning F#
*)
open System.Diagnostics


let dataDrive = "D:\\"
let username = "Administrator"

let shellExecute str = 
    ProcessStartInfo(FileName=str,UseShellExecute=true) |> Process.Start
let shellExecuteNoConsole str = 
    ProcessStartInfo(FileName=str,UseShellExecute=true, WindowStyle=ProcessWindowStyle.Hidden) |> Process.Start

type Address = 
    | LocalFS of string
    | URL of string
    | App of string
    member this.execute() =
        match this with
        | App str -> shellExecuteNoConsole str
        | URL str -> shellExecute str
        | LocalFS str -> 
            ProcessStartInfo(
                FileName="explorer.exe", 
                Arguments = str,
                UseShellExecute=true
            ) |> Process.Start
        |> ignore

type Command = 
    | Go of Address option
    | Launch of string option
    member this.execute() =
        match this with
        | Go(Some addr) -> addr.execute()
        | Launch(Some str) -> shellExecuteNoConsole str |> ignore
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
    match strs with
    | head::tail -> 
        match head with
        // Programming tools
        | "vs" -> Some @"C:\Program Files\Microsoft Visual Studio\2022\Preview\Common7\IDE\devenv.exe"
        | "vsc" | "code" -> Some "code"
        | "npp" -> Some @"C:\Program Files\Notepad++\notepad++.exe"
        // Game Platforms
        | "ubi" | "ubisoft" -> Some @"C:\Program Files (x86)\Ubisoft\Ubisoft Game Launcher\UbisoftConnect.exe"
        | "steam" -> Some @"C:\Program Files (x86)\Steam\steam.exe"
        // Games
        | "ass" -> 
            match tail with
            | "origin"::_ -> Some "uplay://launch/3539/0"
            | "rogue"::_ -> Some "uplay://launch/895/0"
            | "odyssey"::_ -> Some "uplay://launch/5059/0"
            | "black"::_ -> Some "uplay://launch/273/0"
            | _ -> None
        | "teardown" -> Some "steam://rungameid/1167630"
        // Reader
        | "winds" | "wind" -> Some @"C:\Users\rswor\AppData\Local\Programs\Winds\Winds.exe"
        // IM & Social Medias
        | "tg" | "telegram" -> Some @$"C:\Users\{username}\AppData\Roaming\Telegram Desktop\Telegram.exe"
        // Office
        | "word" -> Some @"C:\Program Files\Microsoft Office\root\Office16\WINWORD.EXE"
        | "ppt" | "powerpoint" -> Some @"C:\Program Files\Microsoft Office\root\Office16\POWERPNT.EXE"
        | "onenote" -> Some  @"C:\Program Files\Microsoft Office\root\Office16\ONENOTE.EXE"
        | "excel" -> Some @"C:\Program Files\Microsoft Office\root\Office16\EXCEL.EXE"
        // Markdown
        | "typora" -> Some @"C:\Program Files\Typora\Typora.exe"
        | "notion" -> Some @$"C:\Users\{username}\AppData\Local\Programs\Notion\Notion.exe"
        // Adobe
        | "ps" | "photoshop" -> Some @"C:\Program Files\Adobe Photoshop CC 2018\Photoshop.exe"
        | "pr" | "premiere" -> Some @"C:\Program Files\Adobe Premiere Pro CC 2018\Adobe Premiere Pro.exe"
        | "lr" | "lightroom" -> Some @"C:\Program Files\Adobe Lightroom Classic CC\Lightroom.exe"
        | "ai" | "illustrator" -> Some @"C:\Program Files\Adobe Illustrator CC 2018\Support Files\Contents\Windows\Illustrator.exe"
        | "ae" | "aftereffects" | "afftereffect" | "afterfx" -> Some "C:\Program Files\Adobe After Effects CC 2018\Support Files\AfterFX.exe"
        | "au" | "audition" -> Some @"C:\Program Files\Adobe Audition CC 2018\Adobe Audition CC.exe"
        // Proxy
        | "clash" -> Some @$"C:\Users\{username}\AppData\Local\Programs\Clash for Windows\Clash for Windows.exe"
        | "qv2ray" -> Some @"C:\Program Files\qv2ray\qv2ray.exe"
        // Networking
        | "fiddler" -> Some @$"C:\Users\{username}\AppData\Local\Programs\Fiddler\Fiddler.exe"
        | "shark" | "wireshark" -> Some @"C:\Program Files\Wireshark\Wireshark.exe" 
        // Not matched
        | _ -> None
    | [] -> None

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
Available commands: go(goto), launch(app/start)        
"""
    0 // return an integer exit code