﻿using Microsoft.Build.Framework;
using Microsoft.Build.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace Buildalyzer.Logging
{
    internal class BuildEventArgsReaderProxy
    {
        private readonly Func<BuildEventArgs> _read;

        public BuildEventArgsReaderProxy(BinaryReader reader)
        {
            // Use reflection to get the Microsoft.Build.Logging.BuildEventArgsReader.Read() method
            object argsReader;
            Type buildEventArgsReader = typeof(BinaryLogger).GetTypeInfo().Assembly.GetType("Microsoft.Build.Logging.BuildEventArgsReader");
            ConstructorInfo readerCtor = buildEventArgsReader.GetConstructor(new[] { typeof(BinaryReader) });
            if (readerCtor != null)
            {
                argsReader = readerCtor.Invoke(new[] { reader });
            }
            else
            {
                readerCtor = buildEventArgsReader.GetConstructor(new[] { typeof(BinaryReader), typeof(int) });
                argsReader = readerCtor.Invoke(new object[] { reader, 7 });
            }
            MethodInfo readMethod = buildEventArgsReader.GetMethod("Read");
            _read = (Func<BuildEventArgs>)readMethod.CreateDelegate(typeof(Func<BuildEventArgs>), argsReader);
        }

        public BuildEventArgs Read() => _read();
    }
}
