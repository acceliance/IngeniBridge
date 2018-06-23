using IngeniBridge.Core;
using IngeniBridge.Core.MetaHelper;
using IngeniBridge.Core.Mining;
using IngeniBridge.Core.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCompanyDataModel
{
    public class DatavizIndexHelper : IDatavizIndexHelper
    {
        public string IndexNode ( MetaHelper helper, IteratedNode<object> inode )
        {
            StringBuilder ret = new StringBuilder ();
            inode.Parents.All ( asset =>
            {
                EntityMetaDescription emd = helper.GetMetaDataFromType ( asset.GetType () );
                ret.Append ( emd.EntityDisplayName + " - " );
                ret.Append ( helper.RetrieveCodeValue ( asset ) + " - " );
                ret.Append ( helper.RetrieveLabelValue ( asset ) + " - " );
                return ( true );
            } );
            ret.Append ( inode.vemdNode.EntityDisplayName + " - " );
            ret.Append ( helper.RetrieveCodeValue ( inode.Node ) + " - " );
            ret.Append ( helper.RetrieveLabelValue ( inode.Node ) + " - " );
            helper.ParseAttributes ( inode.Node, ( AttributeMetaDescription attribute, object val ) =>
            {
                if ( val.GetType ().IsSubclassOf ( typeof ( Nomenclature ) ) || val.GetType ().IsSubclassOf ( typeof ( Asset ) ) ) ret.Append ( helper.RetrieveLabelValue ( val ) + " - " );
                else if ( attribute.IsEnum == true ) ret.Append ( val.ToString () + " - " );
                return ( true );
            }, true, true );
            if ( ret.Length > 3 ) ret.Length = ret.Length - 3;
            return ( ret.ToString () );
        }
    }
}
