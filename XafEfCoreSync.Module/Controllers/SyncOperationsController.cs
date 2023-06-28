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
            await node.PullAsync();
        }
        private async void Push_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
          
            await  node.PushAsync();
            
        }
        EFCoreObjectSpace currentEfObjectSpace;
        ISyncClientNode node;
        protected override void OnActivated()
        {
            base.OnActivated();
            currentEfObjectSpace = this.ObjectSpace as EFCoreObjectSpace;
            node = currentEfObjectSpace.DbContext as ISyncClientNode;

            // Perform various tasks depending on the target View.
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
