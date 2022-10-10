﻿using Core.Librarys;
using Core.Librarys.Image;
using Core.Models;
using Core.Servicers.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using UI.Controls;
using UI.Controls.Charts.Model;
using UI.Models;
using UI.Servicers;
using UI.Views;

namespace UI.ViewModels
{
    public class IndexPageVM : IndexPageModel
    {
        public Command ToDetailCommand { get; set; }
        private readonly IData data;
        private readonly MainViewModel main;
        private readonly IMain mainServicer;
        private readonly IAppContextMenuServicer appContextMenuServicer;

        public IndexPageVM(
            IData data, MainViewModel main, IMain mainServicer, IAppContextMenuServicer appContextMenuServicer)
        {
            this.data = data;
            this.main = main;
            this.mainServicer = mainServicer;
            this.appContextMenuServicer = appContextMenuServicer;

            ToDetailCommand = new Command(new Action<object>(OnTodetailCommand));

            Init();
        }


        public override void Dispose()
        {
            PropertyChanged -= IndexPageVM_PropertyChanged;
            base.Dispose();
        }

        private void Init()
        {
            TabbarData = new System.Collections.ObjectModel.ObservableCollection<string>()
            {
                "上周","本周"
            };

            TabbarSelectedIndex = 1;

            PropertyChanged += IndexPageVM_PropertyChanged;

            LoadThisWeekData();

            appContextMenuServicer.Init();
            AppContextMenu = appContextMenuServicer.GetContextMenu();
        }

        private void OnTodetailCommand(object obj)
        {
            var data = obj as ChartsDataModel;

            if (data != null)
            {
                var model = data.Data as DailyLogModel;
                if (model != null && model.AppModel != null)
                {
                    main.Data = model.AppModel;
                    main.Uri = nameof(DetailPage);
                }

            }
        }

        private void IndexPageVM_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(TabbarSelectedIndex))
            {
                if (TabbarSelectedIndex == 1)
                {
                    LoadThisWeekData();
                }
                else
                {
                    LoadLastWeekData();
                }
            }
        }

        #region 读取数据

        #region 本周数据
        private void LoadThisWeekData()
        {
            //IsLoading = true;

            //var task = Task.Run(() =>
            // {
            //     var list = data.GetThisWeeklogList();
            //     var res = MapToChartsData(list);
            //     IsLoading = false;
            //     return res;

            // });

            //WeekData = await task;
            IsLoading = true;

            Task.Run(() =>
               {
                   var list = data.GetThisWeeklogList();
                   var res = MapToChartsData(list);
                   IsLoading = false;
                   WeekData = res;
               });



        }
        #endregion


        #region 上周数据
        private void LoadLastWeekData()
        {
            //IsLoading = true;
            //var task = Task.Run(() =>
            // {
            //     var list = data.GetLastWeeklogList();
            //     var res = MapToChartsData(list);
            //     IsLoading = false;
            //     return res;
            // });
            //WeekData = await task;
            IsLoading = true;
            Task.Run(() =>
            {
                var list = data.GetLastWeeklogList();
                var res = MapToChartsData(list);
                IsLoading = false;
                WeekData = res;
            });

        }
        #endregion

        #region 处理数据
        private List<ChartsDataModel> MapToChartsData(IEnumerable<Core.Models.DailyLogModel> list)
        {
            var resData = new List<ChartsDataModel>();

            foreach (var item in list)
            {
                var bindModel = new ChartsDataModel();
                bindModel.Data = item;
                bindModel.Name = string.IsNullOrEmpty(item.AppModel?.Description) ? item.AppModel.Name : item.AppModel.Description;
                bindModel.Value = item.Time;
                bindModel.Tag = Time.ToString(item.Time);
                bindModel.PopupText = item.AppModel?.File;
                bindModel.Icon = item.AppModel?.IconFile;
                bindModel.DateTime = item.Date;
                resData.Add(bindModel);
            }

            return resData;
        }
        #endregion
        #endregion
    }
}
