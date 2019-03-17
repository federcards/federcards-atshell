
Imports System
Imports BasicCard
Imports BasicCard.Framework
Imports BasicCard.GenericCards


Public Class main

    Public Shared Sub Main()
        Dim card_terminal As CardTerminal
        System.Console.WriteLine("NeoAtlantis Card Interface Start.")

        Try
            CardReaderObserver.Instance.Start()
            card_terminal = CardReaderObserver.Instance.WaitForCardService(
                New CardTerminal(), New TimeSpan(0, 0, 30))
            If card_terminal Is Nothing Then
                System.Console.WriteLine("Press any to exit.")
                Exit Sub
            Else
                card_terminal.start()
            End If



        Catch e As Exception
            System.Console.Error.WriteLine(e.ToString())

        Finally
            CardReaderObserver.Instance.Stop()
        End Try



    End Sub


End Class
