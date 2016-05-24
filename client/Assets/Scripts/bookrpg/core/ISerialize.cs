using UnityEngine;
using System.Collections;

namespace bookrpg.core
{
    public interface ISerialize
    {
        string Serialize() ;

        bool Deserialize(string value) ;
    }
}
