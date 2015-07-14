using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using Kikai.OfferService.Scheduler;

namespace Kikai.OfferService
{
    public partial class OfferService : ServiceBase
    {
        private ISchedulerUtil schdulerUtil = new SchedulerUtil();

        public OfferService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure();
            schdulerUtil.Start();
        }

        protected override void OnStop()
        {
            schdulerUtil.Stop();
        }
    }
}
