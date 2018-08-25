using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IngeniBridge.Server.TestServer
{
    public class NodeInfo
    {
        public string path;
        public string type;
        public Dictionary<string, string> attributes;
        public static NodeInfo RetrieveNodeInfo ( JToken token, StringBuilder path, string type )
        {
            Dictionary<string, string> attributes = new Dictionary<string, string> ();
            return ( AddTokenNodes ( token, null, null, type, null, path ) );
        }
        static NodeInfo AddTokenNodes ( JToken token, string attribute, Stack<string> objects, string type, Dictionary<string, string> attributes, StringBuilder path )
        {
            NodeInfo ni = null;
            if ( path != null )
            {
                objects = new Stack<string> ();
                attributes = new Dictionary<string, string> ();
            }
            if ( token is JValue && ( ( JValue ) token ).Value != null )
            {
                string attr_ = "";
                objects.All ( o => { attr_ = attr_ + o + "."; return ( true ); } );
                attr_ = attr_ + attribute;
                string value = ( ( JValue ) token ).Value.ToString ();
                attributes.Add ( attr_, value );
            }
            else if ( token is JArray )
            {
                AddArrayNodes ( ( JArray ) token, attribute, objects, type, attributes );
            }
            else if ( token is JObject )
            {
                AddObjectNodes ( ( JObject ) token, attribute, objects, type, attributes );
            }
            if ( path != null )
            {
                path.Append ( "\\" + attributes [ "Code" ] );
                ni = new NodeInfo () { path = path.ToString (), type = type, attributes = attributes };
            }
            return ( ni );
        }
        static void AddObjectNodes ( JObject @object, string attribute, Stack<string> objects, string type, Dictionary<string, string> attributes )
        {
            if ( attribute != null ) objects.Push ( attribute );
            foreach ( JProperty property in @object.Properties () )
            {
                AddTokenNodes ( property.Value, property.Name, objects, type, attributes, null );
            }
            if ( attribute != null ) objects.Pop ();
        }
        static void AddArrayNodes ( JArray array, string attribute, Stack<string> objects, string type, Dictionary<string, string> attributes )
        {
            for ( var i = 0; i < array.Count; i++ )
            {
                AddTokenNodes ( array [ i ], attribute + i.ToString (), objects, type, attributes, null );
            }
        }

    }

}
