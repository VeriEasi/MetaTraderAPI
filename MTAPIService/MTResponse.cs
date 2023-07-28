using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Collections;
using System.Globalization;

namespace MTAPIService
{
    [DataContract]
    [KnownType("GetKnownTypes")]
    public abstract class MTResponse
    {
        static IEnumerable<Type> GetKnownTypes()
        {
            return new Type[] { typeof(MTResponseObject),
                                typeof(MTResponseInt), typeof(MTResponseDouble),
                                typeof(MTResponseString), typeof(MTResponseBool),
                                typeof(MTResponseLong), typeof(MTResponseULong),
                                typeof(MTResponseDoubleArray), typeof(MTResponseIntArray),
                                typeof(MTResponseLongArray),
                                typeof(MTResponseArrayList), typeof(MTResponseMQLRatesArray)};
        }

        public abstract object GetValue();

        [DataMember]
        public int ErrorCode { get; set; }
    }

    [DataContract]
    public class MTResponseObject : MTResponse
    {
        public MTResponseObject(object value)
        {
            Value = value;
        }

        [DataMember]
        public object Value { get; private set; }

        public override object GetValue() { return Value; }

        public override string ToString()
        {
            return Value?.ToString() ?? string.Empty;
        }
    }

    [DataContract]
    public class MTResponseInt: MTResponse
    {
        public MTResponseInt(int value)
        {
            Value = value;
        }

        [DataMember]
        public int Value { get; private set; }

        public override object GetValue() { return Value; }

        public override string ToString()
        {
            return Value.ToString();
        }
    }

    [DataContract]
    public class MTResponseLong : MTResponse
    {
        public MTResponseLong(long value)
        {
            Value = value;
        }

        [DataMember]
        public long Value { get; private set; }

        public override object GetValue() { return Value; }

        public override string ToString()
        {
            return Value.ToString();
        }
    }

    [DataContract]
    public class MTResponseULong : MTResponse
    {
        public MTResponseULong(ulong value)
        {
            Value = value;
        }

        [DataMember]
        public ulong Value { get; private set; }

        public override object GetValue() { return Value; }

        public override string ToString()
        {
            return Value.ToString();
        }
    }

    [DataContract]
    public class MTResponseDouble : MTResponse
    {
        public MTResponseDouble(double value)
        {
            Value = value;
        }

        [DataMember]
        public double Value { get; private set; }

        public override object GetValue() { return Value; }

        public override string ToString()
        {
            return Value.ToString(CultureInfo.CurrentCulture);
        }
    }

    [DataContract]
    public class MTResponseString : MTResponse
    {
        public MTResponseString(string value)
        {
            Value = value;
        }

        [DataMember]
        public string Value { get; private set; }

        public override object GetValue() { return Value; }

        public override string ToString()
        {
            return Value;
        }
    }

    [DataContract]
    public class MTResponseBool : MTResponse
    {
        public MTResponseBool(bool value)
        {
            Value = value;
        }

        [DataMember]
        public bool Value { get; private set; }

        public override object GetValue() { return Value; }

        public override string ToString()
        {
            return Value.ToString();
        }
    }

    [DataContract]
    public class MTResponseDoubleArray : MTResponse
    {
        public MTResponseDoubleArray(double[] value)
        {
            Value = value;
        }

        [DataMember]
        public double[] Value { get; private set; }

        public override object GetValue() { return Value; }

        public override string ToString()
        {
            return Value.ToString();
        }
    }

    [DataContract]
    public class MTResponseIntArray : MTResponse
    {
        public MTResponseIntArray(int[] value)
        {
            Value = value;
        }

        [DataMember]
        public int[] Value { get; private set; }

        public override object GetValue() { return Value; }

        public override string ToString()
        {
            return Value.ToString();
        }
    }

    [DataContract]
    public class MTResponseLongArray : MTResponse
    {
        public MTResponseLongArray(long[] value)
        {
            Value = value;
        }

        [DataMember]
        public long[] Value { get; private set; }

        public override object GetValue() { return Value; }

        public override string ToString()
        {
            return Value.ToString();
        }
    }

    [DataContract]
    public class MTResponseArrayList : MTResponse
    {
        public MTResponseArrayList(ArrayList value)
        {
            Value = value;
        }

        [DataMember]
        public ArrayList Value { get; private set; }

        public override object GetValue() { return Value; }

        public override string ToString()
        {
            return Value.ToString();
        }
    }

    [DataContract]
    public class MTResponseMQLRatesArray : MTResponse
    {
        public MTResponseMQLRatesArray(MTMQLRates[] value)
        {
            Value = value;
        }

        [DataMember]
        public MTMQLRates[] Value { get; private set; }

        public override object GetValue() { return Value; }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}
