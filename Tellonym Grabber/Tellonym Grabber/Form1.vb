Imports System.IO
Imports System.Text
Imports System.Net
Imports System.Threading
Imports Microsoft.VisualBasic.CompilerServices, System.Text.RegularExpressions
''' instagram @404.erroz
'''github : https://github.com/itserrozz
Public Class Form1
    Dim sttop As Boolean
    Dim bb As Boolean
    Dim pp As Point
    Dim token As String
    Dim attempts As Integer
    Public Sub New()
        Me.attempts = 0
        InitializeComponent()
    End Sub
    Private Sub Label3_MouseDown(sender As Object, e As MouseEventArgs) Handles Label3.MouseDown
        If e.Button = MouseButtons.Left Then
            Me.pp = New Point(0 - e.X, 0 - e.Y)
            Me.bb = True
        End If
    End Sub

    Private Sub Label3_MouseMove(sender As Object, e As MouseEventArgs) Handles Label3.MouseMove
        If Me.bb Then
            Dim mousePosition As Point = Control.MousePosition
            mousePosition.Offset(Me.pp.X, Me.pp.Y)
            MyBase.Location = mousePosition
        End If
    End Sub

    Private Sub Label3_MouseUp(sender As Object, e As MouseEventArgs) Handles Label3.MouseUp
        If e.Button = MouseButtons.Left Then
            Me.bb = False
        End If
    End Sub
    Private Sub TextBox2_TextChanged(sender As Object, e As EventArgs) Handles TextBox2.TextChanged
        If Not TextBox2.Text = "Password" Then
            TextBox2.UseSystemPasswordChar = True
        Else
            TextBox2.UseSystemPasswordChar = False
        End If
    End Sub
    Public Function login(username As String, password As String) As Object
        Try
            Dim utf As New UTF8Encoding()
            Dim bytes As Byte() = utf.GetBytes("{""deviceName"":""Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:78.0) Gecko/20100101 Firefox/78.0"",""deviceType"":""web"",""lang"":""en"",""captcha"":""03AGdBq25_lFdGVNvBoCdEKv2tbXuDctiPorxexi4wuKyYbgz2hSF1rdhE6QBu2-KQ53_-b-nyJIKwXLCz8LL_xp2hQ5WaRU0jcmjxCAbCs-96X1ZfC0JcEd7ZrINjoQwMl6yRGAiLRW6FZUxl3NmnfU8aHwj2fTEGg8D1ZBjYQPnrfGVi1FQX7vvAkLpSGk3y3sriFSUrrjruHEDrdL4pDxJmmYJkVmIJVnhSDjgKGVvLgpTBlcR_vexnv6zvHQX89YGnWcOcp-c-gnqB3eTDaQoV2YVVMu7N2bObkpWQcBhJIG-5UxD5dAgEoI_DHbiNSTkNBrdzfGzPQWwLRHh8HSLNfMtiOEKKBJvufcq8T0G7Yj1FlSmCBGDsGZnekrNf-oF3NioeLrWW"",""email"":""" & username & """,""password"":""" & password & """,""limit"":25}")
            Dim httpwebrequest As HttpWebRequest = CType(WebRequest.Create("https://api.tellonym.me/tokens/create"), HttpWebRequest)
            httpwebrequest.Method = "POST"
            httpwebrequest.Proxy = Nothing
            httpwebrequest.Host = "api.tellonym.me"
            httpwebrequest.UserAgent = "Mozilla/5.0 (Linux; Android 7.1.1; G8231 Build/41.2.A.0.219; wv) AppleWebKit/537.36 (KHTML, like Gecko) Version/4.0 Chrome/59.0.3071.125 Mobile Safari/537.36"
            httpwebrequest.KeepAlive = True
            httpwebrequest.ContentType = "application/json;charset=utf-8"
            httpwebrequest.ContentLength = CLng(bytes.Length)
            Dim requestStream As Stream = httpwebrequest.GetRequestStream()
            requestStream.Write(bytes, 0, bytes.Length)
            requestStream.Close()
            Dim text As String = New StreamReader(httpwebrequest.GetResponse().GetResponseStream()).ReadToEnd()
            Me.token = Regex.Match(text, """accessToken"":""(.*?)"",""").Groups(1).Value
            If text.Contains("accessToken") Then
                Label2.Text = "Status : login Successfully"
            End If
        Catch ex As WebException
            ProjectData.SetProjectError(ex)
            Dim ex2 As WebException = ex
            Dim textt2 As String = (String.Concat(New StreamReader(ex2.Response.GetResponseStream).ReadToEnd))
            If textt2.Contains("Wrong credentials") Then
                Label2.Text = "Status : Password incorrect!"
            End If

        End Try
    End Function
    Public Sub get_userID()
        If Not token = Nothing Then
            Label2.Text = "Status : Starting .."
        Else
            MsgBox("Please Login First!", MsgBoxStyle.Critical)
        End If
        Try
            Dim httpwebrequest As HttpWebRequest = CType(WebRequest.Create("https://api.tellonym.me/profiles/name/" + TextBox4.Text), HttpWebRequest)
            httpwebrequest.Method = "GET"
            httpwebrequest.Proxy = Nothing
            httpwebrequest.Host = "api.tellonym.me"
            httpwebrequest.UserAgent = "Tellonym/377 CFNetwork/758.5.3 Darwin/15.6.0"
            httpwebrequest.KeepAlive = True
            httpwebrequest.ContentType = "application/json;charset=utf-8"
            Dim httpresponse As HttpWebResponse
            httpresponse = DirectCast(httpwebrequest.GetResponse(), HttpWebResponse)
            Dim reader As New StreamReader(httpresponse.GetResponseStream())
            Dim text As String = reader.ReadToEnd()
            Dim userID As String = Regex.Match(text, "id"":(.*?),").Groups(1).Value
            Dim a As Thread = New Thread(AddressOf get_followers)
            a.Start(userID)
        Catch ex As WebException
            ProjectData.SetProjectError(ex)
            Dim ex2 As WebException = ex
            Dim text_error As String = (String.Concat(New StreamReader(ex2.Response.GetResponseStream).ReadToEnd))
            If text_error.Contains("NOT_FOUND") Then
                MsgBox("Not Found Username!")
            ElseIf text_error.Contains("This account is banned.") Then
                MsgBox("Username Banned !")
            End If

        End Try
    End Sub
    Public Sub get_followers(userID As String)

        Try
            Dim httpwebrequest As HttpWebRequest = CType(WebRequest.Create("https://api.tellonym.me/followers/id/" + userID + "?userId=" + userID + "&adExpId=82&limit=16"), HttpWebRequest)
            httpwebrequest.Method = "GET"
            httpwebrequest.Proxy = Nothing
            httpwebrequest.Host = "api.tellonym.me"
            httpwebrequest.UserAgent = "Tellonym/377 CFNetwork/758.5.3 Darwin/15.6.0"
            httpwebrequest.KeepAlive = True
            httpwebrequest.Headers.Add("authorization", "Bearer " & token)
            httpwebrequest.ContentType = "application/json;charset=utf-8"
            Dim httpresponse As HttpWebResponse
            httpresponse = DirectCast(httpwebrequest.GetResponse(), HttpWebResponse)
            Dim reader As New StreamReader(httpresponse.GetResponseStream())
            Dim text As String = reader.ReadToEnd()
            If text.Contains("{""followers"":[]}") Then
                MsgBox("Don't have Followers!")
            Else

                Dim user_regex As MatchCollection = Regex.Matches(text, "username"":""(.*?)"",")
                For Each obj As Object In user_regex
                    Dim id As String = Regex.Match(text, "id"":(.*?),").Groups(1).Value
                    Dim value_json As Match = CType(obj, Match)
                    Dim user As String = value_json.Groups(1).Value.ToString()
                    TextBox3.AppendText(user & vbCrLf)
                    attempts += 1
                    Label1.Text = "Users : " + attempts.ToString()
                    My.Computer.FileSystem.WriteAllText("Usernames.txt", user + vbCrLf, True)
                Next
                MsgBox("Finshed ! " + attempts.ToString)
            End If
        Catch ex As WebException
            ProjectData.SetProjectError(ex)
            Dim ex2 As WebException = ex
            Dim text_error As String = (String.Concat(New StreamReader(ex2.Response.GetResponseStream).ReadToEnd))
            MsgBox(text_error, MsgBoxStyle.Critical)
        End Try
    End Sub
    Public Sub random_grab()
        While True
            If Not token = Nothing Then
                Label2.Text = "Status : Starting .."
            Else
                MsgBox("Please Login First!", MsgBoxStyle.Critical)
                Exit While
            End If
            Thread.Sleep(1500)
            Try
                Dim httpwebrequest As HttpWebRequest = CType(WebRequest.Create("https://api.tellonym.me/suggestions/people"), HttpWebRequest)
                httpwebrequest.Method = "GET"
                httpwebrequest.Proxy = Nothing
                httpwebrequest.Host = "api.tellonym.me"
                httpwebrequest.UserAgent = "Tellonym/377 CFNetwork/758.5.3 Darwin/15.6.0"
                httpwebrequest.KeepAlive = True
                httpwebrequest.Headers.Add("authorization", "Bearer " & token)
                httpwebrequest.ContentType = "application/json;charset=utf-8"
                Dim httpresponse As HttpWebResponse
                httpresponse = DirectCast(httpwebrequest.GetResponse(), HttpWebResponse)
                Dim reader As New StreamReader(httpresponse.GetResponseStream())
                Dim text As String = reader.ReadToEnd()
                Dim user_regex As MatchCollection = Regex.Matches(text, "username"":""(.*?)"",")
                For Each obj As Object In user_regex
                    Dim id As String = Regex.Match(text, "id"":(.*?),").Groups(1).Value
                    Dim tcg As Match = CType(obj, Match)
                    Dim user As String = tcg.Groups(1).Value.ToString()
                    TextBox3.AppendText(user & vbCrLf)
                    My.Computer.FileSystem.WriteAllText("Usernames.txt", user + vbCrLf, True)
                    attempts += 1
                    Label1.Text = "Users : " + attempts.ToString()
                Next
            Catch ex As WebException
                ProjectData.SetProjectError(ex)
                Dim ex2 As WebException = ex
                Dim text_error As String = (String.Concat(New StreamReader(ex2.Response.GetResponseStream).ReadToEnd))
                If text_error.Contains("RATELIMIT_REACHED") Then
                    MsgBox("Finshed ! " + attempts.ToString)
                    Exit While
                End If
            End Try
        End While
    End Sub
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim user As String = TextBox1.Text
        Dim password As String = TextBox2.Text
        login(user, password)
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Control.CheckForIllegalCrossThreadCalls = False
        MsgBox("welcome .." + vbCrLf + "instagram : @404.erroz" + vbCrLf + "github : itserrozz", MsgBoxStyle.Information, "Codded By @404.erroz")
    End Sub
    Private Sub Label4_Click(sender As Object, e As EventArgs) Handles Label4.Click
        ProjectData.EndApp()
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        If RadioButton1.Checked = True Then
            Dim a As Thread = New Thread(AddressOf random_grab)
            a.Start()
        Else
            Dim a2 As Thread = New Thread(AddressOf get_userID)
            a2.Start()
        End If
    End Sub
End Class
