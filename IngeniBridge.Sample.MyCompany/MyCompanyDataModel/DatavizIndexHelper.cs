using IngeniBridge.Core;
using IngeniBridge.Core.Iterator;
using IngeniBridge.Core.MetaHelper;
using IngeniBridge.Core.Mining;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCompanyDataModel
{
    // *********************************************************
    // If you want to activate DataViz REST service, just enable the script below
    // This script must produce a text describing each TimeData node
    // Every description will be full indexed and can be searched theought DataViz REST service
    // *********************************************************
    //
    //public class DatavizIndexHelper : IDatavizIndexHelper
    //{
    //    public string IndexNode ( MetaHelper helper, IteratedNode<object> inode )
    //    {
    //        StringBuilder ret = new StringBuilder ();
    //        inode.Parents.All ( asset =>
    //        {
    //            EntityMetaDescription emd = helper.GetMetaDataFromType ( asset.GetType () );
    //            ret.Append ( emd.EntityDisplayName + " - " );
    //            ret.Append ( helper.RetrieveCodeValue ( asset ) + " - " );
    //            ret.Append ( helper.RetrieveLabelValue ( asset ) + " - " );
    //            return ( true );
    //        } );
    //        ret.Append ( inode.vemdNode.EntityDisplayName + " - " );
    //        ret.Append ( helper.RetrieveCodeValue ( inode.Node ) + " - " );
    //        ret.Append ( helper.RetrieveLabelValue ( inode.Node ) + " - " );
    //        helper.ParseAttributes ( inode.Node, ( AttributeMetaDescription attribute, object val ) =>
    //        {
    //            if ( val.GetType ().IsSubclassOf ( typeof ( Nomenclature ) ) || val.GetType ().IsSubclassOf ( typeof ( Asset ) ) ) ret.Append ( helper.RetrieveLabelValue ( val ) + " - " );
    //            else if ( attribute.IsEnum == true ) ret.Append ( val.ToString () + " - " );
    //            return ( true );
    //        }, true, true );
    //        return ( ret.ToString () );
    //    }
    //}
}
