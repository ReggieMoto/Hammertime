using System;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections;
using System.Security;

namespace Hammertime
{
    public class TeamopolisReader
    {
        private string _teamopolisUrl;
        private string _teamopolisSurveyUrl;

        // =====================================================
        public TeamopolisReader(string url = "http://madhockeynh.teamopolis.com")
        // =====================================================
        {
            _teamopolisUrl = url;
        }

        // =====================================================
        //  Read in the player profile page using the URL, extract
        //  the player's name from the page, and return it
        // =====================================================
        private string SurveyResponderName(string url)
        // =====================================================
        {
            string responderName=null;
            string containsPattern = ">Name<";
            string firstSubStringPattern = "<td class=\"headlineSmText\">Name</td><td class=\"regText\">";
            string lastSubStringPattern = "</td>";
            string pageLine = null;


            // Open a stream to point to the data stream coming from the Teamopolis Player Profile Web resource.
            try
            {
                var webRequest = (HttpWebRequest)WebRequest.Create(url);
                webRequest.CookieContainer = new CookieContainer();

                try
                {
                    // Get the response.
                    HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse();
                    // Get the stream containing content returned by the server.
                    Stream dataStream = webResponse.GetResponseStream();
                    // Open the stream using a StreamReader for easy access.
                    StreamReader myPlayerProfileReader = new StreamReader(dataStream);

                    do
                    {
                        try
                        {
                            // Get a line of source text from the HTML file
                            pageLine = myPlayerProfileReader.ReadLine();
                            // Check to see whether it contains the survey URL
                            if (pageLine != null && pageLine.Contains(containsPattern))
                            {
                                int first = pageLine.IndexOf(firstSubStringPattern) + firstSubStringPattern.Length;
                                int last = pageLine.Length - lastSubStringPattern.Length;
                                responderName = pageLine.Substring(first, last - first);
                            }
                        }
                        catch (IOException ex)
                        {
                            Console.WriteLine($"Error reading {url}: {ex.Message}");
                        }

                    } while (pageLine != null);

                    myPlayerProfileReader.Close();
                    dataStream.Close();
                    webResponse.Close();

                }
                catch (WebException ex)
                {
                    Console.WriteLine($"Error opening {url}: {ex.Message}");
                }
            }
            catch (UriFormatException ex)
            {
                Console.WriteLine($"Error opening {url}: {ex.Message}");
            }
            catch (SecurityException ex)
            {
                Console.WriteLine($"Error opening {url}: {ex.Message}");
            }

            return responderName;
        }

        // =====================================================
        private bool FindSurveyResponders(string pageLine, out ArrayList surveyResponderUrls)
        // =====================================================
        {
            bool match = false;
            string pattern = @"(/members/roster\.aspx\?tmid=[0-9]+)";
            string responderUrl;

            surveyResponderUrls = null;

            foreach (Match m in Regex.Matches(pageLine, pattern))
            {
                if (match == false)
                {
                    match = true;
                    surveyResponderUrls = new ArrayList();
                }

                responderUrl = _teamopolisUrl + m.Groups[1].Value;
                surveyResponderUrls.Add(responderUrl);
            }

            return match;
        }

        // =====================================================
        //  Only interested in those Full Time or Sub responders
        //  who are "In for this Monday", so we will read only
        //  those player URLs and skip the rest.
        //
        //  The lists are arranged like this:
        //  1. (Full-Time Players) I'm In for this Monday!
        //  2. (Full-Time Players) Sorry Boy's I'm Out!
        //  3. (I am a Sub) I'm Available! Call me if you need me!
        //  4. Others we aren't interested in.
        //
        //  So, gather responders from lists 1 and 3.
        // =====================================================
        private void TeamopolisSurveyResponders()
        // =====================================================
        {
            var webClient = new WebClient();
            ArrayList surveyResponderUrls;
            ArrayList availablePlayerUrls = new ArrayList();
            int responderType = 0;

            // Open a stream to point to the data stream coming from the Teamopolis Survey Web resource.
            try
            {
                Stream mySurveyStream = webClient.OpenRead(_teamopolisSurveyUrl);
                StreamReader mySurveyReader = new StreamReader(mySurveyStream);

                string pageLine;
                do
                {
                    // Get a line of source text from the HTML file
                    pageLine = mySurveyReader.ReadLine();
                    // Check to see whether it contains the survey URL
                    if (pageLine != null)
                    {
                        if (FindSurveyResponders(pageLine, out surveyResponderUrls))
                        {
                            if (responderType == 0 || responderType == 2)
                            {
                                foreach (string url in surveyResponderUrls)
                                    availablePlayerUrls.Add(url);
                            }

                            responderType++; // Keep the index current, whether we use it or not
                        }
                    }

                } while (pageLine != null);

                mySurveyStream.Close();
                mySurveyReader.Close();

                Console.WriteLine("Available Players:");
                int counter = 1;
                foreach (string url in availablePlayerUrls)
                    Console.WriteLine($"{counter++}. {SurveyResponderName(url)}");
                    //Console.WriteLine($"{counter++}. {url}");

                Console.WriteLine();
            }
            catch (WebException ex)
            {
                Console.WriteLine($"Error opening Teamopolis Survey URL: {ex.Message}");
            }

            // _teamopolisSurveyResponders = surveyResponderUrls;
        }

        // =====================================================
        private string TeamopolisSurveyUrl(string pageLine)
        // =====================================================
        {
            string pattern = @"(/surveyvote\.aspx\?a=1&amp;SID=[0-9]+)";
            string url = null;

            foreach (Match m in Regex.Matches(pageLine, pattern))
            {
                // Remove some HTML formatting caca
                pattern = "amp;";
                url = m.Groups[1].Value;
                int index1 = url.IndexOf(pattern);
                url = url.Remove(index1, pattern.Length);
                // Prepend the Teamopolis URL
                url = _teamopolisUrl + url;
            }

            return url;
        }

        // =====================================================
        public void TeamopolisSurveyResults()
        // =====================================================
        {
            var webClient = new WebClient(); // For communicating with the Teamopolis site

            // Open a stream to point to the data stream coming from the Teamopolis Web resource.
            try
            {
                Stream myStream = webClient.OpenRead(_teamopolisUrl);
                StreamReader myReader = new StreamReader(myStream);

                string pageLine;
                do
                {
                    // Get a line of source text from the HTML file
                    pageLine = myReader.ReadLine();
                    // Check to see whether it contains the survey URL
                    if (pageLine != null)
                        _teamopolisSurveyUrl = TeamopolisSurveyUrl(pageLine);

                } while (pageLine != null && _teamopolisSurveyUrl == null);

                myStream.Close();
                myReader.Close();

                if (_teamopolisSurveyUrl != null)   // Did we find the survey URL?
                    TeamopolisSurveyResponders();   // If so, go build a list of responders

            }
            catch (WebException ex)
            {
                Console.WriteLine($"Error opening Teamopolis URL: {ex.Message}");
            }
        }
    }
}