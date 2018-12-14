﻿using Ray.Core.Messaging;
using System;
using System.IO;
using ProtoBuf;

namespace Ray.IGrains
{
    public class ProtobufSerializer : ISerializer
    {
        public object Deserialize(Type type, Stream source)
        {
            return Serializer.Deserialize(type, source);
        }

        public T Deserialize<T>(Stream source)
        {
            return Serializer.Deserialize<T>(source);
        }

        public void Serialize<T>(Stream destination, T instance)
        {
            Serializer.Serialize(destination, instance);
        }
    }
}
