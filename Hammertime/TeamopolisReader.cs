// ==============================================================
//
// class TeamopolisReader
//
// Copyright (c) 2017 David Hammond 
// All Rights Reserved.
// 
// ==============================================================
// NOTICE:  All information contained herein is and remains the
// property of David Hammond. The intellectual and technical
// concepts contained herein are proprietary to David Hammond
// and may be covered by U.S.and Foreign Patents, patents in
// process, and are protected by trade secret or copyright law.
// Dissemination of this information or reproduction of this
// material is strictly forbidden unless prior written permission
// is obtained David Hammond.
// ==============================================================

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
        // ==============================================================
        public static TeamopolisReader Instance
        // ==============================================================
        {
            get
            {
                if (_teamopolisReader == null)
                    _teamopolisReader = new TeamopolisReader();
                return _teamopolisReader;
            }
        }

        private static TeamopolisReader _teamopolisReader;  // Reader instance
        private ArrayList _teamopolisAvailablePlayers;       // Available survey responders
        private string _teamopolisUrl;
        private string _teamopolisSurveyUrl;


        public ArrayList AvailablePlayers
        {
                get { return _teamopolisAvailablePlayers; }
        }

        // =====================================================
        private TeamopolisReader(string url = "http://madhockeynh.teamopolis.com")
        // =====================================================
        {
            _teamopolisUrl = url;
            _teamopolisAvailablePlayers = new ArrayList();
            TeamopolisSurveyResults();
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
                                _teamopolisAvailablePlayers.Add(responderName);
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
        //  0. (Full-Time Players) I'm In for this Monday!
        //  1. (Full-Time Players) Sorry Boy's I'm Out!
        //  2. (I am a Sub) I'm Available! Call me if you need me!
        //  3. Others we aren't interested in.
        //
        //  So, gather responders from lists 1 and 3.
        // =====================================================
        private void TeamopolisSurveyResponders()
        // =====================================================
        {
            var webClient = new WebClient();
            string[] responderTypePatterns =
            {
                "(I'm In)",         // (Full-Time Players) I'm In for this Monday!
                "(I'm Out)",        // (Full-Time Players) Sorry Boy's I'm Out!
                "(I'm Available)",  // (I am a Sub) I'm Available! Call me if you need me!
                "(I'm not)"         // (I am a Sub) Sorry, I'm not available this Week.
            };
            ArrayList surveyResponderUrls;
            ArrayList availablePlayerUrls = new ArrayList();
            int responderType = -1;

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
                    if (pageLine != null)
                    {
                        // Before we search for responder names let's find out what kind of responders they might be
                        // We are only interested in responder types 0 (Full-time and available) and 2 (Sub and available)
                        // The responderType will serve as an index into the responderTypePatterns string array.
                        // If a match occurs then we will keep parsing, looking for the survey responders
                        Match match = Regex.Match(pageLine, responderTypePatterns[responderType+1]);
                        if (match.Groups[1].Value != "")
                        {
                            responderType++;
                            //Console.WriteLine($"{responderTypePatterns[responderType]}");
                        }

                        if ((responderType == 0 || responderType == 2) &&
                            FindSurveyResponders(pageLine, out surveyResponderUrls))
                        {
                            foreach (string url in surveyResponderUrls)
                                availablePlayerUrls.Add(url);
                        }
                    }

                } while (pageLine != null && responderType < 3);

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
        private void TeamopolisSurveyResults()
        // =====================================================
        {
            if (HammerMain.ReadSurveyResults == false)
            {
                try
                {
                    _teamopolisAvailablePlayers = AvailablePlayerFileIO.Instance.ReadAvailablePlayers();
                }
                catch (AvailablePlayerFileIOException ex)
                {
                    Console.WriteLine($"Error reading file of available players: {ex.Message}");
                }
            }
            else
            {
                var webClient = new WebClient(); // For communicating with the Teamopolis site

                // Open a stream to point to the data stream coming from the Teamopolis Web resource.
                try
                {
                    Stream myStream = webClient.OpenRead(_teamopolisUrl);
                    StreamReader myReader = new StreamReader(myStream);

                    //Console.WriteLine("Retrieving Teamopolis survey responders.");
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

                    // Now that we have the list locally, in memory, write it to a local file
                    AvailablePlayerFileIO.Instance.WriteAvailablePlayers(_teamopolisAvailablePlayers);
                }
                catch (WebException ex)
                {
                    Console.WriteLine($"Error opening Teamopolis URL: {ex.Message}");
                }
            }
        }
    }
}