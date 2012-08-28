﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using Autofac.Integration.Mvc;
using ZCMS.Core.Business.Content;

namespace ZCMS.Core.Business
{
    [ModelBinderType(typeof(ZCMSPage))]
    public class ZCMSPageModelBinder : IModelBinder
    {

        public ZCMSPageModelBinder()
        {
        }
        
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {

            ZCMSPage model = (ZCMSPage)Activator.CreateInstance(bindingContext.ModelType);
            //model.StartPublish = null;
            //model.EndPublish = null;

            //var items = bindingContext.ValueProvider.GetValue("Properties").AttemptedValue;
            model.PageName = bindingContext.ValueProvider.GetValue("PageName").AttemptedValue;
            //var valMenue = bindingContext.ValueProvider.GetValue("ShowInMenus").AttemptedValue;
            //model.ShowInMenus = Convert.ToBoolean(bindingContext.ValueProvider.GetValue("ShowInMenus").AttemptedValue.Split(',')[0]);
            //model.UrlSlug = bindingContext.ValueProvider.GetValue("UrlSlug").AttemptedValue;
            model.PageType = bindingContext.ValueProvider.GetValue("PageType").AttemptedValue;
            //model.AllowComments = Convert.ToBoolean(bindingContext.ValueProvider.GetValue("AllowComments").AttemptedValue.Split(',')[0]);
            try
            {
                model.PageID = Int32.Parse(bindingContext.ValueProvider.GetValue("PageID").AttemptedValue);
            }
            catch
            {
                model.PageID = new Random().Next(100, 100000);
            }

            if (bindingContext.ValueProvider.GetValue("save-draft") != null)
                model.Status = PageStatus.Draft;
            else
                model.Status = PageStatus.Published;

            DateTime spd, epd;
            //bool sp = DateTime.TryParse(bindingContext.ValueProvider.GetValue("StartPublish").AttemptedValue, out spd);
            //if (sp)
            //    model.StartPublish = spd;

            //bool ep = DateTime.TryParse(bindingContext.ValueProvider.GetValue("EndPublish").AttemptedValue, out epd);
            //if (ep)
            //    model.EndPublish = epd;

            var items2 = bindingContext.ValueProvider.GetValue("Properties[0].PropertyName").AttemptedValue;

            // limiting number of properties to 8 on purpose
            for (int i = 0; i < 12; i++)
            {
                try
                {

                    var type = bindingContext.ValueProvider.GetValue("Properties["+ i +"].PropertyType").AttemptedValue;
                    if (type == "ZCMS.Core.Business.Content.RichTextProperty")
                    {
                        RichTextProperty prop = new RichTextProperty();
                        prop.PropertyName = bindingContext.ValueProvider.GetValue("Properties[" + i + "].PropertyName").AttemptedValue;
                        prop.PropertyValue = bindingContext.ValueProvider.GetValue("PropertyValue").AttemptedValue;
                        prop.Order = Int32.Parse(bindingContext.ValueProvider.GetValue("Properties[" + i + "].Order").AttemptedValue);
                        if (bindingContext.ValueProvider.GetValue("Properties[" + i + "].PropertyValidator")!=null)
                            prop.PropertyValidator = bindingContext.ValueProvider.GetValue("Properties[" + i + "].PropertyValidator").AttemptedValue;
                        else
                            prop.PropertyValidator = string.Empty;
                        model.Properties.Add(prop);
                    }
                    else if (type == "ZCMS.Core.Business.Content.BooleanProperty")
                    {
                        BooleanProperty prop = new BooleanProperty();
                        prop.PropertyName = bindingContext.ValueProvider.GetValue("Properties[" + i + "].PropertyName").AttemptedValue;
                        var val = bindingContext.ValueProvider.GetValue("Properties[" + i + "].PropertyValue").AttemptedValue;
                        if(val.Split(',').Length>1)
                            prop.PropertyValue = Convert.ToBoolean(bindingContext.ValueProvider.GetValue("Properties[" + i + "].PropertyValue").AttemptedValue.Split(',')[0]);
                        else
                            prop.PropertyValue = Convert.ToBoolean(bindingContext.ValueProvider.GetValue("Properties[" + i + "].PropertyValue").AttemptedValue);
                        prop.Order = Int32.Parse(bindingContext.ValueProvider.GetValue("Properties[" + i + "].Order").AttemptedValue);
                        model.Properties.Add(prop);
                    }
                    else if (type == "ZCMS.Core.Business.Content.DisplayOnlyTextProperty")
                    {
                        DisplayOnlyTextProperty prop = new DisplayOnlyTextProperty();
                        prop.PropertyName = bindingContext.ValueProvider.GetValue("Properties[" + i + "].PropertyName").AttemptedValue;
                        prop.PropertyValue = bindingContext.ValueProvider.GetValue("Properties[" + i + "].PropertyValue").AttemptedValue;
                        prop.Order = Int32.Parse(bindingContext.ValueProvider.GetValue("Properties[" + i + "].Order").AttemptedValue);
                        model.Properties.Add(prop);
                    }
                    else if (type == "ZCMS.Core.Business.Content.DateProperty")
                    {
                        DateProperty prop = new DateProperty();
                        prop.PropertyName = bindingContext.ValueProvider.GetValue("Properties[" + i + "].PropertyName").AttemptedValue;
                        string val = string.Empty;
                        if(!String.IsNullOrEmpty(bindingContext.ValueProvider.GetValue("Properties[" + i + "].PropertyValue").AttemptedValue))
                            prop.PropertyValue = Convert.ToDateTime(bindingContext.ValueProvider.GetValue("Properties[" + i + "].PropertyValue").AttemptedValue);
                        else 
                            prop.PropertyValue = null;

                        prop.Order = Int32.Parse(bindingContext.ValueProvider.GetValue("Properties[" + i + "].Order").AttemptedValue);
                        if (bindingContext.ValueProvider.GetValue("Properties[" + i + "].PropertyValidator")!=null)
                            prop.PropertyValidator = bindingContext.ValueProvider.GetValue("Properties[" + i + "].PropertyValidator").AttemptedValue;
                        else
                            prop.PropertyValidator = string.Empty;
                        model.Properties.Add(prop);
                    }
                    else if (type == "ZCMS.Core.Business.Content.TagsProperty")
                    {
                        TagsProperty tp = new TagsProperty();
                        tp.PropertyName = bindingContext.ValueProvider.GetValue("Properties[" + i + "].PropertyName").AttemptedValue;
                        tp.Order = Int32.Parse(bindingContext.ValueProvider.GetValue("Properties[" + i + "].Order").AttemptedValue);
                        tp.PropertyValue = new List<string>();
                        try
                        {
                            string[] tagvalues = bindingContext.ValueProvider.GetValue("PropertyValue.PageTagValues").AttemptedValue.Split(',');
                            List<string> tags = new List<string>();
                            foreach (string s in tagvalues)
                            {
                                if (s != null && s != string.Empty)
                                    tags.Add(s);
                            }
                            tp.PropertyValue = tags;

                        }
                        catch
                        {
                        }
                        model.Properties.Add(tp);
                    }
                    else
                    {
                        var propInstance = Activator.CreateInstance(Type.GetType(bindingContext.ValueProvider.GetValue("Properties[" + i + "].PropertyType").AttemptedValue));

                        var propInstanceProperties = propInstance.GetType().GetProperties();

                        foreach (PropertyInfo propInfo in propInstanceProperties)
                        {
                            try
                            {
                                var value = bindingContext.ValueProvider.GetValue("Properties[" + i + "]." + propInfo.Name).AttemptedValue;
                                if (propInfo.PropertyType.FullName == "System.Int32")
                                    propInfo.SetValue(propInstance, Int32.Parse(value));
                                else if (propInfo.PropertyType.FullName == "FluentValidation.IValidator")
                                    propInfo.SetValue(propInstance, Activator.CreateInstance(Type.GetType(value)));
                                else
                                    propInfo.SetValue(propInstance, value);
                            }
                            catch
                            {
                            }
                        }

                        model.Properties.Add((IZCMSProperty)propInstance);
                    }
                }
                catch
                {
                    break;
                }
            }
            model.Properties.OrderBy(o => o.Order);
            return model;
        }
    }
}