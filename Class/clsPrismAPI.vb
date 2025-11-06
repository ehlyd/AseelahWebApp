Imports System.IO
Imports System.Net
Imports System.Text
Imports Microsoft.Reporting.Map.WebForms.BingMaps
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports WebGrease.Activities

Public Class clsPrismAPI

    Dim authNonce As String, authNonceResponse As String

    Dim intConnectTries As Integer
    Dim invPostedDate As String

    Public authSession As String
    Public EmpSID As String

    Dim APIUrl As String, APIUserName As String, APIPassword As String

    Public Sub New()
        APIUrl = ConfigurationManager.AppSettings("ServerURL")
    End Sub

    Public Function IsAPI_LoginSuccessfull(UserID As String, Password As String) As Boolean
        Try

ReTry:      authNonce = GetAuthNonce()

            If authNonce <> "" Then

                authNonceResponse = Get_AuthNonceResponse(authNonce)

                authSession = GetAuthSession(UserID, Password)

                If authSession.Contains("(401) Unauthorized") Then

                    If intConnectTries < 3 Then

                        GoTo ReTry
                    Else
                        'Return "Unable to get Auth-Session"
                        'Throw New Exception("Unable to get Auth-Session after 3 tries. Please check the API credentials.")
                        Throw New Exception("Unable to login. Server.")
                        'Return False
                    End If

                Else

                    Dim str1stSession As String
                    str1stSession = Get1stSession()

                    If str1stSession.Contains("200") Then
                        Dim strWebClient As String
                        strWebClient = GetWebClient()

                        If strWebClient.Contains("200") Then
                            Dim str3rdSession As String
                            str3rdSession = Get3rdSession()

                            If str3rdSession.Contains("200") Then

                                'Return "Authorized"
                                Return True

                            Else
                                'Return "Unable to get third session"
                                Throw New Exception("Unable to get third session.")
                            End If

                        Else
                            'Return "Unable to get Workstation"
                            Throw New Exception("Unable to get Workstation.")
                        End If

                    Else
                        'Return "Unable to get first session"
                        Throw New Exception("Unable to get first session.")
                    End If

                End If

            Else
                Return False
            End If

        Catch ex As Exception
            Throw ex
        End Try

    End Function


    Function GetAuthSession(APIUserName As String, APIPassword As String) As String
        Try
            intConnectTries += 1
            Dim url As String = APIUrl & "/v1/rest/auth?usr=" & APIUserName & "&pwd=" & APIPassword

            Dim request As HttpWebRequest = WebRequest.Create(url)

            request.Method = "GET"
            request.Headers.Add("Auth-Nonce", authNonce)
            request.Headers.Add("Auth-Nonce-Response", authNonceResponse)

            request.ServicePoint.ConnectionLimit = 10
            request.ServicePoint.MaxIdleTime = 5 * 1000
            request.Timeout = 60000

            Dim response As HttpWebResponse = request.GetResponse()

            If response.StatusCode = HttpStatusCode.OK Then

                Dim jsonLoginResponse As String = ""
                Using stream As Stream = response.GetResponseStream()
                    Using reader As StreamReader = New StreamReader(stream)
                        jsonLoginResponse = reader.ReadToEnd()
                    End Using
                End Using

                Dim jsonArray As JArray = JArray.Parse(jsonLoginResponse)

                If jsonArray.Count > 0 Then
                    Dim firstObject As JObject = CType(jsonArray(0), JObject)

                    Dim employeesidToken As JToken = firstObject("employeesid")

                    If employeesidToken IsNot Nothing Then EmpSID = employeesidToken.ToString()
                End If

                Dim header As String = response.Headers.Get("Auth-Session")
                Return header
            Else
                Return ""
            End If

            response.Close()

        Catch ex As WebException

            If ex.Message.Contains("(401) Unauthorized") Then
                Dim jsonErrResponse As String = ""
                Dim jsonObject As JObject

                Using stream As Stream = ex.Response.GetResponseStream()
                    Using reader As StreamReader = New StreamReader(stream)
                        jsonErrResponse = reader.ReadToEnd()
                    End Using
                End Using

                jsonObject = JObject.Parse(jsonErrResponse)
                Dim Errmsg As String = jsonObject("errors").ToString()

                If Errmsg.Contains("Account is locked") Then
                    Throw New Exception("Account is locked. Please contact IT.")
                ElseIf Errmsg.Contains("403: Invalid username or password") Then
                    Throw New Exception("Invalid username or password!")
                Else
                    Return "(401) Unauthorized"
                End If

            Else
                Throw ex
            End If

        End Try

    End Function

    Function Get1stSession() As String
        Dim url As String = APIUrl & "/v1/rest/session"

        Try
            Dim request As HttpWebRequest = WebRequest.Create(url)
            request.Headers.Add("Auth-Session", authSession)
            request.Method = "GET"

            request.ServicePoint.ConnectionLimit = 10
            request.ServicePoint.MaxIdleTime = 5 * 1000
            request.Timeout = 60000

            Dim response As HttpWebResponse = request.GetResponse()

            If response.StatusCode = HttpStatusCode.OK Then
                Return response.StatusCode
            Else
                Return "Error " & response.StatusCode
            End If

            response.Close()
        Catch ex As Exception
            Throw ex
        End Try
    End Function

    Function GetWebClient() As String
        Dim url As String = APIUrl & "/v1/rest/sit?ws=webclient" '& currWokstation

        Try
            Dim request As HttpWebRequest = WebRequest.Create(url)
            request.Headers.Add("Auth-Session", authSession)
            request.Method = "GET"

            request.ServicePoint.ConnectionLimit = 10
            request.ServicePoint.MaxIdleTime = 5 * 1000
            request.Timeout = 60000

            Dim response As HttpWebResponse = request.GetResponse()

            If response.StatusCode = HttpStatusCode.OK Then
                Return response.StatusCode
            Else
                Return "Error " & response.StatusCode
            End If

            response.Close()
        Catch ex As Exception
            Throw ex
        End Try
    End Function

    Public Sub LogoutWebClient()
        Dim url As String = APIUrl & "/api/security/logout?ws=webclient" '& currWokstation

        Try
            Dim request As HttpWebRequest = WebRequest.Create(url)
            request.Headers.Add("Auth-Session", authSession)
            request.ContentType = "application/json"
            request.Method = "POST"
            request.Accept = "application/json, text/plain, version=2"

            request.ServicePoint.ConnectionLimit = 10
            request.ServicePoint.MaxIdleTime = 5 * 1000
            request.Timeout = 60000

            Dim response As HttpWebResponse = request.GetResponse()

            If response.StatusCode = HttpStatusCode.OK Then
                'authSession = ""
                Console.WriteLine("Logged out successfully.")
            End If

            response.Close()
        Catch ex As Exception
            Throw ex
        End Try
    End Sub



    Function Get3rdSession() As String
        Dim url As String = APIUrl & "/v1/rest/session"

        Try
            Dim request As HttpWebRequest = WebRequest.Create(url)
            request.Headers.Add("Auth-Session", authSession)
            request.Method = "GET"

            request.ServicePoint.ConnectionLimit = 10
            request.ServicePoint.MaxIdleTime = 5 * 1000
            request.Timeout = 60000

            Dim response As HttpWebResponse = request.GetResponse()

            If response.StatusCode = HttpStatusCode.OK Then

                Dim responseStream As Stream = response.GetResponseStream()
                Dim reader As New StreamReader(responseStream)

                ' Read response content
                Dim responseContent As String = reader.ReadToEnd()

                Dim responseObject As Object = JsonConvert.DeserializeObject(responseContent)

                'For Each dataObject In responseObject
                '    currStoreSid = Replace(dataObject("storesid").ToString, """", "")
                '    currStoreCode = Replace(dataObject("storecode").ToString, """", "")
                'Next

                Return response.StatusCode
            Else
                Return "Error " & response.StatusCode
            End If

            response.Close()
        Catch ex As Exception
            Throw ex
        End Try
    End Function


    Function GetAuthNonce() As String
        Try

            Dim url As String = APIUrl & "/v1/rest/auth"
            Dim request As HttpWebRequest = WebRequest.Create(url)

            request.Method = "GET"
            request.ServicePoint.ConnectionLimit = 10
            request.ServicePoint.MaxIdleTime = 5 * 1000
            request.Timeout = 60000

            Dim response As HttpWebResponse = request.GetResponse()
            If response.StatusCode = HttpStatusCode.OK Then
                Dim header As String = response.Headers.Get("Auth-Nonce")
                Return header
            Else
                Return ""
            End If

            response.Close()

        Catch ex As Exception
            Throw ex
        End Try
    End Function

    Function GetAuthNonce1() As String
        Try

            Dim url As String = APIUrl & "/v1/rest/auth"
            Dim request As WebRequest = HttpWebRequest.Create(url)

            ' Set request method
            request.Method = "GET"
            request.Timeout = 60000

            Dim response As WebResponse = request.GetResponse()
            Dim headers As WebHeaderCollection = response.Headers

            ' Check if Auth-Nonce header exists
            If headers.AllKeys.Contains("Auth-Nonce") Then
                Return headers("Auth-Nonce")
            Else
                Return ""
            End If

            response.Close()

        Catch ex As Exception
            Throw ex
        End Try
    End Function

    Function Get_AuthNonceResponse(ByVal lngAuthNonce As Long) As Long
        Try
            Dim dbldiv13 As Double, mod59 As Double

            dbldiv13 = Math.Round(lngAuthNonce / 13, 0)
            mod59 = dbldiv13 Mod 99999

            Return mod59 * 17

        Catch ex As Exception
            Throw ex
        End Try
    End Function

    Private Function ReadJsonFromFile(ByVal filePath As String) As String
        Try
            Using reader As New StreamReader(filePath)
                Return reader.ReadToEnd()
            End Using
        Catch ex As Exception
            Throw ex
        End Try
    End Function

    Private Function ReadResponseStream(ByVal response As HttpWebResponse) As String
        Try
            Dim reader As New StreamReader(response.GetResponseStream())
            Dim responseString As String = reader.ReadToEnd()
            reader.Close()
            Return responseString
        Catch ex As Exception
            Throw
        End Try

    End Function

    Public Function CreatePI(storeSID As String, PIName As String, PINote As String) As String
        Try

            Dim PI_SID As String = ""

            Dim json As String = "[
                                      {
                                        ""origin_application"": ""RProPrismWeb"",
                                        ""store_sid"": """ & storeSID & """,
                                        ""name"": """ & PIName & """,
                                        ""note"": """ & PINote & """
                                      }
                                   ]"

            Dim url As String = APIUrl & "/v1/rest/pisheet"

            Dim request As HttpWebRequest = WebRequest.Create(url)
            request.Method = "POST"
            request.ContentType = "application/json"
            request.Accept = "application/json, text/plain, version=2"
            request.Headers.Add("Auth-Session", authSession)

            request.ServicePoint.ConnectionLimit = 10
            request.ServicePoint.MaxIdleTime = 5 * 1000
            request.Timeout = 60000


            Dim byteArray As Byte() = Encoding.UTF8.GetBytes(json)
            request.ContentLength = byteArray.Length

            Dim dataStream As Stream = request.GetRequestStream()
            dataStream.Write(byteArray, 0, byteArray.Length)
            dataStream.Close()

            Try

                Dim response As HttpWebResponse = CType(request.GetResponse(), HttpWebResponse)
                Dim responseString As String = ""

                If response.StatusCode = HttpStatusCode.Created Then

                    responseString = ReadResponseStream(response)
                    Dim responseObject As Object = JsonConvert.DeserializeObject(responseString)

                    For Each dataObject In responseObject
                        PI_SID = Replace(dataObject("sid").ToString, """", "")
                    Next

                    InitializePI(PI_SID)

                    Return PI_SID

                Else
                    Throw New Exception("Error creating PI sheet: " & response.StatusCode & " " & response.StatusDescription)
                End If

                response.Close()

            Catch webex As WebException
                Throw webex
            End Try

        Catch ex As Exception
            Throw
        End Try
    End Function

    Private Sub InitializePI(PISheedSID As String)
        Try

            Dim json As String = "[
                                      {
                                        ""MethodName"": ""PIInitPISheetMethod"",
                                        ""Params"": {
                                          ""PISheetSID"": """ & PISheedSID & """
                                        }
                                      }
                                    ]"

            Dim url As String = APIUrl & "/v1/rpc/"

            Dim request As HttpWebRequest = WebRequest.Create(url)
            request.Method = "POST"
            request.ContentType = "application/json"
            request.Accept = "application/json, text/plain, version=2"
            request.Headers.Add("Auth-Session", authSession)

            request.ServicePoint.ConnectionLimit = 10
            request.ServicePoint.MaxIdleTime = 5 * 1000
            request.Timeout = 60000


            Dim byteArray As Byte() = Encoding.UTF8.GetBytes(json)
            request.ContentLength = byteArray.Length

            Dim dataStream As Stream = request.GetRequestStream()
            dataStream.Write(byteArray, 0, byteArray.Length)
            dataStream.Close()

            Try

                Dim response As HttpWebResponse = CType(request.GetResponse(), HttpWebResponse)
                Dim responseString As String = ""

                If response.StatusCode = HttpStatusCode.OK Then

                    'responseString = ReadResponseStream(response)
                    'Dim responseObject As Object = JsonConvert.DeserializeObject(responseString)

                Else
                    Throw New Exception("Error initializing PI sheet: " & response.StatusCode & " " & response.StatusDescription)
                End If

                response.Close()

            Catch webex As WebException
                Throw webex
            End Try

        Catch ex As Exception
            Throw
        End Try
    End Sub

    Public Sub PIMerge(PIZoneSID As String)
        Try

            Dim json As String = "[
                                      {
                                        ""Params"": {
                                          ""PIZoneSID"": """ & PIZoneSID & """,
                                          ""QtyType"": 2,
                                          ""AllowDiscrepancies"": 0
                                        },
                                        ""MethodName"": ""PIMergeQty""
                                      }
                                    ]"

            Dim url As String = APIUrl & "/v1/rpc/"

            Dim request As HttpWebRequest = WebRequest.Create(url)
            request.Method = "POST"
            request.ContentType = "application/json"
            request.Accept = "application/json, text/plain, version=2"
            request.Headers.Add("Auth-Session", authSession)

            request.ServicePoint.ConnectionLimit = 10
            request.ServicePoint.MaxIdleTime = 5 * 1000
            request.Timeout = 60000


            Dim byteArray As Byte() = Encoding.UTF8.GetBytes(json)
            request.ContentLength = byteArray.Length

            Dim dataStream As Stream = request.GetRequestStream()
            dataStream.Write(byteArray, 0, byteArray.Length)
            dataStream.Close()

            Try

                Dim response As HttpWebResponse = CType(request.GetResponse(), HttpWebResponse)
                Dim responseString As String = ""

                If response.StatusCode = HttpStatusCode.OK Then

                    'responseString = ReadResponseStream(response)
                    'Dim responseObject As Object = JsonConvert.DeserializeObject(responseString)

                Else
                    Throw New Exception("Error merging PI sheet: " & response.StatusCode & " " & response.StatusDescription)
                End If

                response.Close()

            Catch webex As WebException
                Throw webex
            End Try

        Catch ex As Exception
            Throw
        End Try
    End Sub

    Public Sub ImportPIFile(PISheedSID As String, mapSID As String, fileBytes As Byte())
        Try


            Dim url As String = APIUrl & "/v1/rest/piimport?SID=" & PISheedSID & "&zonesid=null&mapsid=" & mapSID

            Dim request As HttpWebRequest = WebRequest.Create(url)
            request.Method = "POST"
            'request.ContentType = "application/json"
            request.Accept = "application/json, text/plain, version=2"
            request.Headers.Add("Auth-Session", authSession)

            request.ServicePoint.ConnectionLimit = 10
            request.ServicePoint.MaxIdleTime = 5 * 1000
            request.Timeout = 60000

            Dim byteArray As Byte() = fileBytes
            request.ContentLength = byteArray.Length

            Dim dataStream As Stream = request.GetRequestStream()
            dataStream.Write(byteArray, 0, byteArray.Length)
            dataStream.Close()

            Try

                Dim response As HttpWebResponse = CType(request.GetResponse(), HttpWebResponse)
                Dim responseString As String = ""

                If response.StatusCode = HttpStatusCode.OK Then

                    'responseString = ReadResponseStream(response)
                    'Dim responseObject As Object = JsonConvert.DeserializeObject(responseString)

                Else
                    Throw New Exception("Error importing PI file: " & response.StatusCode & " " & response.StatusDescription)
                End If

                response.Close()

            Catch webex As WebException
                Throw webex
            End Try

        Catch ex As Exception
            Throw
        End Try
    End Sub


End Class
