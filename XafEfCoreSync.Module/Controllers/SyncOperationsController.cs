using BIT.Data.Sync.Client;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.EFCore;
using DevExpress.ExpressApp.Layout;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using XafEfCoreSync.Module.BusinessObjects;

namespace XafEfCoreSync.Module.Controllers
{
    // For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/clsDevExpressExpressAppViewControllertopic.aspx.
    public partial class SyncOperationsController : ViewController
    {
        SimpleAction Pull;
        SimpleAction Push;
        // Use CodeRush to create Controllers and Actions with a few keystrokes.
        // https://docs.devexpress.com/CodeRushForRoslyn/403133/
        public SyncOperationsController()
        {
            InitializeComponent();
            this.TargetObjectType = typeof(Blog);
            Push = new SimpleAction(this, "Push", "View");
            Push.Execute += Push_Execute;
            Pull = new SimpleAction(this, "Pull", "View");
            Pull.Execute += Pull_Execute;
            

            // Target required Views (via the TargetXXX properties) and create their Actions.
        }
        private async void Pull_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            GetNode();
            await node.PullAsync();
            ShowMessage("Pull...Done!");
            refreshController.RefreshAction.DoExecute();
        }
        private async void Push_Execute(object sender, SimpleActionExecuteEventArgs e)
        {

            GetNode();
            //var Deltas=await node.DeltaStore.GetDeltasAsync(Guid.Empty, CancellationToken.None);

            var LastPushedDelta = await node.DeltaStore.GetLastPushedDeltaAsync(node.Identity, CancellationToken.None).ConfigureAwait(false);
            var Deltas = await node.DeltaStore.GetDeltasByIdentityAsync(LastPushedDelta, node.Identity, CancellationToken.None).ConfigureAwait(false);
            if (Deltas.Any())
            {
                var Max = Deltas.Max(d => d.Index);
                await node.SyncFrameworkClient.PushAsync(Deltas, CancellationToken.None).ConfigureAwait(false);
                await node.DeltaStore.SetLastPushedDeltaAsync(Max, node.Identity, CancellationToken.None).ConfigureAwait(false);
            }


            //await  node.PushAsync();
            ShowMessage("Push...Done!");


        }
        void ShowMessage(string Message)
        {
            MessageOptions options = new MessageOptions();
            options.Duration = 2000;
            options.Message = Message;
            options.Type = InformationType.Success;
            options.Web.Position = InformationPosition.Right;
            options.Win.Caption = "Success";
            options.Win.Type = WinMessageType.Toast;
            options.OkDelegate = () => {
                
            };
            Application.ShowViewStrategy.ShowMessage(options);
        }
        EFCoreObjectSpace currentEfObjectSpace;
        ISyncClientNode node;
        RefreshController refreshController;
        protected override void OnActivated()
        {
            base.OnActivated();
            GetNode();
            refreshController = this.Frame.GetController<RefreshController>();
            // Perform various tasks depending on the target View.
        }

        private void GetNode()
        {
            currentEfObjectSpace = this.Application.CreateObjectSpace(typeof(Blog)) as EFCoreObjectSpace;
            node = currentEfObjectSpace.DbContext as ISyncClientNode;
        }

        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            // Access and customize the target View control.
        }
        protected override void OnDeactivated()
        {
            // Unsubscribe from previously subscribed events and release other references and resources.
            base.OnDeactivated();
        }
    }
}
