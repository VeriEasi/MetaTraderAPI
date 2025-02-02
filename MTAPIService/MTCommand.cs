﻿using System.Runtime.Serialization;
using System.Collections;
using System.Collections.Generic;

namespace MTAPIService
{
    [DataContract]
    public class MTCommand
    {
        [DataMember]
        public int CommandType { get; set; }

        [DataMember]
        public ArrayList Parameters { get; set; }

        [DataMember]
        public Dictionary<string, object> NamedParams { get; set; }

        [DataMember]
        public int ExpertHandle { get; set; }

        public override string ToString()
        {
            return $"CommandType = {CommandType}; ExpertHandle = {ExpertHandle}";
        }
    }
}
