﻿using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Controls.WpfPropertyGrid.Attributes;
using Hawk.Core.Connectors;
using Hawk.Core.Utils;
using Hawk.Core.Utils.MVVM;
using Hawk.Core.Utils.Plugins;
using Hawk.ETL.Interfaces;

namespace Hawk.ETL.Plugins.Transformers
{

    public abstract class TransformerBase : PropertyChangeNotifier, IColumnDataTransformer
    {
        #region Constants and Fields

        #endregion

        #region Constructors and Destructors
        protected bool IsExecute;

        public void SetExecute(bool value)
        {
            IsExecute = value;
        }
        protected TransformerBase()
        {
            this.OneOutput = true;
            this.Column = "";
            this.NewColumn = "";
            this.Enabled = true;
            IsMultiYield = false;

        }

        #endregion

        #region Properties



        [LocalizedCategory("1.基本选项"), PropertyOrder(1), DisplayName("原列名")]
        [LocalizedDescription("本模块要处理的列的名称")]
        public string Column { get; set; }

        [LocalizedDisplayName("介绍")]
        [PropertyOrder(100)]
        [PropertyEditor("CodeEditor")]
        public string Description
        {
            get
            {
                var item = AttributeHelper.GetCustomAttribute(GetType());
                if (item == null)
                    return GetType().ToString();
                return item.Description;
            }
        }

        [LocalizedCategory("1.基本选项")]
        [LocalizedDisplayName("标签")]
        public string Name { get; set; }


        [LocalizedCategory("1.基本选项")]
        [PropertyOrder(2)]
        [LocalizedDisplayName("新列名")]
        [LocalizedDescription("结果要输出到的列的名称")]
        public virtual string NewColumn { get; set; }

  
        private bool _enabled;

        [LocalizedCategory("1.基本选项")]
        [LocalizedDisplayName("启用")]
        [PropertyOrder(5)]
        public bool Enabled
        {
            get { return _enabled; }
            set
            {
                if (_enabled == value) return;
                _enabled = value;
                OnPropertyChanged("Enabled");
            }
        }


        /// <summary>
        ///     是否在数据中必须包含列名
        /// </summary>
        [Browsable(false)]
        public virtual bool OneOutput { get;  set; }




        [LocalizedCategory("1.基本选项")]
        [LocalizedDisplayName("类型")]
        [PropertyOrder(0)]
        public string TypeName
        {
            get
            {
                XFrmWorkAttribute item = AttributeHelper.GetCustomAttribute(this.GetType());
                return item == null ? this.GetType().ToString() : item.Name;
            }
        }

        #endregion

        #region Public Methods

        public override string ToString()
        {
            return this.TypeName + " " + this.Column;
        }

        #endregion

        #region Implemented Interfaces

        #region IColumnDataTransformer

        public virtual object TransformData(IFreeDocument datas)
        {
            return null;
        }

        [Browsable(false)]
        public virtual bool IsMultiYield { get; set; }


        protected void SetValue(IFreeDocument doc,object item)
        {
            if(string.IsNullOrEmpty(NewColumn))
                doc.SetValue(Column,item);
            else
                doc.SetValue(NewColumn,item);
        }
     

        public virtual IEnumerable<IFreeDocument> TransformManyData(IEnumerable<IFreeDocument> datas)
        {
            yield break;
        }

       
        #endregion

        #region IColumnProcess

        public virtual void Finish()
        {

        }

        public virtual bool Init(IEnumerable<IFreeDocument> docus)
        {
            return true;
        }

        #endregion

        #region IDictionarySerializable

        public virtual void DictDeserialize(IDictionary<string, object> docu, Scenario scenario = Scenario.Database)
        {
            this.UnsafeDictDeserialize(docu);
           
        }

        public virtual FreeDocument DictSerialize(Scenario scenario = Scenario.Database)
        {
            var dict = this.UnsafeDictSerialize();
            dict.Add("Type", this.GetType().Name);
            dict.Add("Group","Transformer");
            return dict;
        }

        #endregion

        #endregion
    }
}