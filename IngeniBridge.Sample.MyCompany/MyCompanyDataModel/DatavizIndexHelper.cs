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
        public string IndexNode ( StorageAccessor accessor, string PathInTree, object node )
        {
            StringBuilder ret = new StringBuilder ();
            string [ ] l = StorageFormatter.GetAllParentsPathFromFullPath ( PathInTree );
            StorageFormatter.GetAllParentsPathFromFullPath ( PathInTree ).All ( parentpath =>
            {
                StorageNode<Asset> parent = accessor.RetrieveEntityFromPath<Asset> ( parentpath );
                EntityMetaDescription emd_ = accessor.MetaHelper.GetMetaDataFromType ( parent.Entity.GetType () );
                ret.Append ( emd_.EntityDisplayName + " - " );
                ret.Append ( accessor.ContentHelper.RetrieveCodeValue ( parent ) + " - " );
                ret.Append ( accessor.ContentHelper.RetrieveLabelValue ( parent ) + " - " );
                return ( true );
            } );
            EntityMetaDescription emd = accessor.MetaHelper.GetMetaDataFromType ( node.GetType () );
            ret.Append ( emd.EntityDisplayName + " - " );
            ret.Append ( accessor.ContentHelper.RetrieveCodeValue ( node ) + " - " );
            ret.Append ( accessor.ContentHelper.RetrieveLabelValue ( node ) + " - " );
            accessor.ContentHelper.ParseAttributes ( node, ( AttributeMetaDescription attribute, object val ) =>
            {
                if ( val.GetType ().IsSubclassOf ( typeof ( Nomenclature ) ) || val.GetType ().IsSubclassOf ( typeof ( Asset ) ) ) ret.Append ( accessor.ContentHelper.RetrieveLabelValue ( val ) + " - " );
                else if ( attribute.IsEnum == true ) ret.Append ( val.ToString () + " - " );
                return ( true );
            }, true, true );
            if ( ret.Length > 3 ) ret.Length = ret.Length - 3;
            return ( ret.ToString () );
        }
    }
}
