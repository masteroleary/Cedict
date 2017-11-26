module DictTests

    open Cedict
    open FsUnit
    open System
    open System.IO
    open System.Text
    open Xunit

    let createDict (lines : string list) =
        String.Join (Environment.NewLine, lines)
            |> Encoding.UTF8.GetBytes
            |> (fun buffer -> new MemoryStream (buffer))
            |> Dict.FromStream

    [<Fact>]
    let ``Dict.FromStream should return Dict`` () =
        createDict []
            |> should not' (be null)

    [<Fact>]
    let ``Dict.FromStream should return empty Dict when there are no entries`` () =
        createDict []
            |> should haveLength 0

    [<Fact>]
    let ``Dict.FromStream should not read invalid entries`` () =
        createDict [""; Environment.NewLine; "Hello"; "你好"]
            |> should haveLength 0

    [<Fact>]
    let ``Dict.FromStream should read valid entries`` () =
        createDict ["你好 你好 [ni3 hao3] /Hello!/Hi!/How are you?/"]
            |> should haveLength 1

    [<Fact>]
    let ``Dict.FromStream should parse valid entries`` () =
        let dict = createDict ["你好 你好 [ni3 hao3] /Hello!/Hi!/How are you?/"];

        let expected = {
            Traditional = "你好"
            Simplified = "你好"
            Pinyin = "ni3 hao3"
            English = [|"Hello!"; "Hi!"; "How are you?"|]
        }

        Seq.exactlyOne dict.Entries
            |> should equal expected
