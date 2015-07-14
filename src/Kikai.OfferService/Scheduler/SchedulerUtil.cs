using Kikai.OfferService.Daemons;
using Quartz;
using Quartz.Impl;
using System;
using System.Configuration;

namespace Kikai.OfferService.Scheduler
{
    public class SchedulerUtil : ISchedulerUtil
    {
        private IScheduler scheduler = null;

        public void Start()
        {
            try
            {
                scheduler = StdSchedulerFactory.GetDefaultScheduler();

                // start scheduler
                scheduler.Start();

                // schedule all the recurring jobs
                StartActiveOffersJob();
                StartPendingOffersJob();
                StartSuspendedOffersJob();

            }
            catch (SchedulerException)
            {
                throw;
            }
        }

        public void Stop()
        {
            try
            {
                if (scheduler != null)
                {
                    scheduler.Shutdown();
                    scheduler = null;
                }
            }
            catch (SchedulerException)
            {
                throw;
            }
        }

        private void StartActiveOffersJob()
        {
            try
            {
                int repeatIntervalsInSeconds = Convert.ToInt32(ConfigurationManager.AppSettings["ActiveOffersJobRetryMinutes"]) * 60;

                // define the job and tie it to our ActiveOffersJob class
                IJobDetail job = JobBuilder.Create<ActiveOffersJob>()
                    .WithIdentity("activeOffers", "group1")
                    .Build();

                // Create trigger for the job, and then repeat based on configurable value
                ITrigger trigger = TriggerBuilder.Create()
                    .WithIdentity("activeTrigger", "group1")
                    .StartNow()
                    .WithSimpleSchedule(x => x
                        .WithIntervalInSeconds(repeatIntervalsInSeconds)
                        .RepeatForever())
                    .Build();

                // schedule the job
                scheduler.ScheduleJob(job, trigger);
            }
            catch (SchedulerException)
            {
                throw;
            }
        }

        private void StartPendingOffersJob()
        {
            try
            {
                int repeatIntervalsInSeconds = Convert.ToInt32(ConfigurationManager.AppSettings["PendingOffersJobRetryMinutes"]) * 60;

                // define the job and tie it to our ActiveOffersJob class
                IJobDetail job = JobBuilder.Create<PendingOffersJob>()
                    .WithIdentity("pendingOffers", "group1")
                    .Build();

                // Create trigger for the job, and then repeat based on configurable value
                ITrigger trigger = TriggerBuilder.Create()
                    .WithIdentity("pendingTrigger", "group1")
                    .StartNow()
                    .WithSimpleSchedule(x => x
                        .WithIntervalInSeconds(repeatIntervalsInSeconds)
                        .RepeatForever())
                    .Build();

                // schedule the job
                scheduler.ScheduleJob(job, trigger);
            }
            catch (SchedulerException)
            {
                throw;
            }
        }

        private void StartSuspendedOffersJob()
        {
            try
            {
                int repeatIntervalsInSeconds = Convert.ToInt32(ConfigurationManager.AppSettings["SuspendedOffersJobRetryMinutes"]) * 60;

                // define the job and tie it to our SuspendedOffersJob class
                IJobDetail job = JobBuilder.Create<SuspendedOffersJob>()
                    .WithIdentity("suspendedOffers", "group1")
                    .Build();

                // Create trigger for the job, and then repeat based on configurable value
                ITrigger trigger = TriggerBuilder.Create()
                    .WithIdentity("suspendedTrigger", "group1")
                    .StartNow()
                    .WithSimpleSchedule(x => x
                        .WithIntervalInSeconds(repeatIntervalsInSeconds)
                        .RepeatForever())
                    .Build();

                // schedule the job
                scheduler.ScheduleJob(job, trigger);
            }
            catch (SchedulerException)
            {
                throw;
            }
        }
    }
}
