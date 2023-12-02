using UnityEngine;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System;
using System.Net;
using TMPro;

public class NodeStructureHandler : MonoBehaviour
{
    // information about THIS node
    public string node_url;
    public bool expanded = false;
    public bool is_cycle = false;
    public int downloadProgress = 0;
    public bool scanning = false;

    public string scanError = null;

    // things that THIS node will need in its "gameplay loop"
    public GameObject node_mould_object;
    private VarHolder vars;
    public List<GameObject> connections;
    public LineRenderer linerenderer;
    public NodePhysicsHandler PhysicsHandler;
    public NodeColorHandler ColorHandler;

    public void OnProgress(object sender, DownloadProgressChangedEventArgs e){
        var k = e.ProgressPercentage;
        this.downloadProgress = k;
        if (k == 100) { // if progress is 100% then scanning is done
            scanning = false;
        }
    }

    public static string pattern = @"<a\s+(?:[^>]*?\s+)?href=""([^""]*)""";

    public void OnComplete(object sender, DownloadStringCompletedEventArgs e){
        // node scanned
        scanning = false;
        if (e.Cancelled || e.Error != null){
            if (e.Error == null) return;
            Debug.LogError(e.Error);
            scanError = e.Error.Message;
            return;
        }
        string html = e.Result;
        List<string> links = new List<string>();

        MatchCollection matches = Regex.Matches(html, pattern, RegexOptions.IgnoreCase);

        foreach (Match match in matches)
        {
            string href = match.Groups[1].Value;

            if (!string.IsNullOrEmpty(href))
            {
                string absoluteUrl = new Uri(new Uri(node_url), href).AbsoluteUri;
                links.Add(absoluteUrl);
            }
        }
        AttachUrls(links);
    }

    // when either the game starts, or a node is created
    private void Awake()
    {
        // just in case
        connections.Clear();
        // reset initial settings
        expanded = false;
        is_cycle = false;

        // get all the components refrenced at the start, so they are all organized
        vars = GameObject.Find("Main Camera").GetComponent<VarHolder>();
        linerenderer = gameObject.GetComponent<LineRenderer>();
        node_mould_object = GameObject.Find("mould");
        PhysicsHandler = gameObject.GetComponent<NodePhysicsHandler>();
        ColorHandler = gameObject.GetComponent<NodeColorHandler>();
  }

    // !! FIRST READ expand_node() THEN READ void update() !!

    private void Update()
    {
        // for every connection this node created (from expand_node))
        for (int x = 0; x < connections.Count; x++)
        {
            // check just in case the connection got deleted or something idk
            if (connections[x] != null)
            {
                // we want to draw a like from position A to position B
                Vector3 positionA = transform.position;
                Vector3 positionB = connections[x].GetComponent<Transform>().position;

                // thats why when we created the number of positions in the linerenderer we did it: number of connections * 2
                // explanation can be found here: ctrl + f "=+=+="
                linerenderer.SetPosition(x * 2, positionA);
                linerenderer.SetPosition(x * 2 + 1, positionB);
            }
            else
            {
                // if something did go wrong, we want to make sure that the line is deleted right?
                // unity is trash so there is no option to delete a specific position from a LineRenderer
                // this i think settting both unwanted positions in the same location works
                linerenderer.SetPosition(x * 2, new Vector3(0, 0, 0));
                linerenderer.SetPosition(x * 2 + 1, new Vector3(0, 0, 0));
            }
        }
    }

    public static string userAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/119.0.0.0 Safari/537.36";

    public void TaskDownload(string url) {
        scanError = null;
        using WebClient webClient = new WebClient();
        webClient.Headers.Add("User-Agent", userAgent);
        // add event listener for download progress
        webClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(OnProgress);
        // add event listener to create links when downloaded
        webClient.DownloadStringCompleted += new DownloadStringCompletedEventHandler(OnComplete);
        webClient.DownloadStringAsync(new Uri(url));
    }

    public bool IsSamePage(string url1, string url2)
    {
        string normalizedUrl1 = NormalizeUrl(url1);
        string normalizedUrl2 = NormalizeUrl(url2);

        return normalizedUrl1 == normalizedUrl2;
    }

    private string NormalizeUrl(string url)
    {
        // Convert to lowercase
        url = url.ToLowerInvariant();

        // Remove scheme (http and https)
        url = url.Replace("http://", "").Replace("https://", "");

        // Remove fragment and query string
        int fragmentIndex = url.IndexOf('#');
        if (fragmentIndex != -1)
            url = url.Substring(0, fragmentIndex);

        int queryIndex = url.IndexOf('?');
        if (queryIndex != -1)
            url = url.Substring(0, queryIndex);

        return url;
    }

    public void AttachUrls(List<string> connected_urls){
        // make sure to update the settings (so player cant spam the same node)
        expanded = true;
        ColorHandler.ColorModeRefreshFlag = -1;
        // after retrieving all of the links connected to THIS node, we do the following operations:
        foreach (string url in connected_urls)
        {
            // check if we stumbled back on THIS node
            // if this happens, that means the node is cyclic - meaning that this webpage has a link going back to itself
            if (!IsSamePage(url, node_url))
            {
                // we dont want any duplicate nodes, so if we find any (from a big string) we dont create a new node, but connect to an existing one
                if (!vars.AllNodeUrls.Contains(url))
                {
                    // if its not a duplicate we found, we create a new node
                    // node mould object is a clone of THIS node, only difference is that its associated URL (node_url) is an empty string
                    // it can be found as a child of the main camera
                    // yes, i am aware prefabs exist im having some mysterious cicumstances happen when using them
                    GameObject current_node = Instantiate(node_mould_object, transform);
                    // give the newborn node a random position (close to THIS node)
                    current_node.transform.position = transform.position + new Vector3(UnityEngine.Random.Range(-vars.InitGenRange, vars.InitGenRange),
                                                                                       UnityEngine.Random.Range(-vars.InitGenRange, vars.InitGenRange),
                                                                                       UnityEngine.Random.Range(-vars.InitGenRange, vars.InitGenRange));
                    // give the newborn its URL (see how it has the same variable node_url as THIS node)
                    // ]][[
                    current_node.GetComponent<NodeStructureHandler>().node_url = url;
                    // name the new node by its url (this is required in this line) --------------|
                    current_node.name = url;              //                                      |
                    // add the new node's url to that big string i was talking about before       |
                    vars.AllNodeUrls += url + " \n ";   //                                        |
                    // add that new node to a list that i call connections                        |
                    // it is used to both draw lines between nodes, and later handle physics      |
                    connections.Add(current_node);        //                                      |
                }                                         //                                      |
                else                                      //                                      |
                {                                         //                                      |
                    connections.Add(GameObject.Find(url));//    <<<-------------------------------|
                }
            }
            else
            {
                is_cycle = true;
            }
        }
        // =+=+=
        // so basically how we draw the lines between nodes?
        // a LineRenderer is something that draws a line between a list of points
        // we cant JUST add all the positions of the child nodes, because then the line will go thru all the children (they might not be connected)
        // so we bounce it back and fourth, from THIS node, to each child
        // thats why the number of points that the linerenderer needs to go thru is double the number of connections
        //          2 child      4 child
        //               ^           ^
        //                \         /
        //                 \       /       *same line renderer
        //                3 V     V 5          goes back and fourth
        //              1  THIS NODE
        //                9 ^     ^ 7
        //                 /       \                       it goes in order: 1 > 2 > 3 > 4 > 5 > 6 > 7 > 8 > 9
        //                /         \
        //               V           V
        //            child 8      child 6
        connections.RemoveAll(item => item == null);
        linerenderer.positionCount = connections.Count * 2;

        // tell the physics script what connections were found
        PhysicsHandler.connections = connections;
    }

    //[0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12]
    // this function will be called if we want to scan THIS node
    public void ExpandNode()
    {
        // check if the node hass already been scanned (so player cant spam the same node)
        if (expanded) return;
        // Start downloading content for parsing
        scanning = true;

        // We only wanna do fancy guesswork when there is no protocol on the URL and when the url is not empty
        if (node_url != null && node_url != "" && !node_url.Contains("://"))
        {

            // Check if there are any slashes which separate the string.
            // We can achieve this by splitting the string by "/" and extrapolating information
            // from the resulting array.
            //
            // If the array is of size 1, that means the string had a traling, leading or no slash at all.
            // If the array is if size 2 or larger then we can safely assume that there are separating slashes
            var separations = new List<string>(node_url.Split('/'));

            if (separations.Count >= 2)
            {

                string host = separations[0];
                separations.Remove(host);
                
                // We now have the host (hopefully)
                // We can look it up on the DNS to see if it's valid
                IPHostEntry dnslookup;
                
                dnslookup = Dns.GetHostEntry(host);

                // If there  is no adresses found then the hostname is invalid
                if (dnslookup.AddressList.Length == 0)
                {
                    scanError = "Invalid hostname";
                    return;
                }
                node_url = $"https://{host}/{String.Join("/", separations)}/";
            }
            else
            {
                node_url = $"https://{separations[0].Trim('/')}/";
            }
   

        }
        
        TaskDownload(node_url);
    }
}
