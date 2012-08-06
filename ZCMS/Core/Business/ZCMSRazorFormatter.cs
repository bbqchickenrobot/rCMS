﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.IO;
using System.Net;
using System.Net.Http.Headers;
using System.Reflection;
using System.Threading.Tasks;
using System;
using System.Net.Http.Formatting;
using RazorEngine;
using System.Collections;

namespace ZCMS.Core.Business
{

    public class ZCMSRazorFormatter : MediaTypeFormatter
    {
        public ZCMSRazorFormatter()
        {
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/xhtml+xml"));
        }

        //...

        public override Task WriteToStreamAsync(
                                                Type type,
                                                object value,
                                                Stream stream,
                                                HttpContentHeaders contentHeaders,
                                                TransportContext transportContext)
        {
            var task = Task.Factory.StartNew(() =>
                {
                    var viewPath = type.Name.Replace("<", "_");// "";// Get path to the view by the name of the type

                    var template = File.ReadAllText(viewPath);

                    Razor.Compile(template, type, type.Name);
                    var razor = Razor.Run(type.Name, value);

                    var buf = System.Text.Encoding.Default.GetBytes(razor);

                    stream.Write(buf, 0, buf.Length);

                    stream.Flush();
                });

            return task;
        }

        public override bool CanWriteType(System.Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            return isTypeOfIEnumerable(type);
        }

        private bool isTypeOfIEnumerable(Type type)
        {

            foreach (Type interfaceType in type.GetInterfaces())
            {

                if (interfaceType == typeof(IEnumerable))
                    return true;
            }

            return false;
        }

        public override bool CanReadType(System.Type type)
        {
            throw new System.NotImplementedException();
        }
    }
}
