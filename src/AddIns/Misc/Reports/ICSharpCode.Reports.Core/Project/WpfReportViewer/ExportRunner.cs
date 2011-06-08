﻿/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 23.05.2011
 * Time: 20:01
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using ICSharpCode.Reports.Core.Exporter;
using ICSharpCode.Reports.Core.Globals;
using ICSharpCode.Reports.Core.ReportViewer;

namespace ICSharpCode.Reports.Core.WpfReportViewer
{
	/// <summary>
	/// Description of WpfReportViewModel.
	/// </summary>
	public class ExportRunner:IPreviewModel
	{
		public ExportRunner()
		{
			Pages = new PagesCollection();
		}
		
		
		public System.Windows.Documents.IDocumentPaginatorSource Document {
			set {
				throw new NotImplementedException();
			}
		}
		
		
		public PagesCollection Pages {get;private set;}
	
		
		public ICSharpCode.Reports.Core.ReportViewer.IReportViewerMessages Messages {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public void RunReport(string fileName, ReportParameters parameters)
		{
			if (String.IsNullOrEmpty(fileName)) {
				throw new ArgumentNullException("fileName");
			}
			ReportModel model = ReportEngine.LoadReportModel(fileName);
			this.RunReport(model, parameters);
		}
		
		public void RunReport(ReportModel reportModel, ReportParameters parameters)
		{
			if (reportModel == null) {
				throw new ArgumentNullException("reportModel");
			}
			Pages.Clear();
			if (reportModel.DataModel == GlobalEnums.PushPullModel.FormSheet)
			{
				RunFormSheet(reportModel);
			} else {
				ReportEngine.CheckForParameters(reportModel, parameters);
				var dataManager = DataManagerFactory.CreateDataManager(reportModel, parameters);
				RunReport(reportModel, dataManager);
			}
		}
		
		public void RunReport(ReportModel reportModel, System.Data.DataTable dataTable, ReportParameters parameters)
		{
				if (reportModel == null) {
				throw new ArgumentNullException("reportModel");
			}
			if (dataTable == null) {
				throw new ArgumentNullException("dataTable");
			}
			ReportEngine.CheckForParameters(reportModel, parameters);
			IDataManager dataManager = DataManagerFactory.CreateDataManager(reportModel, dataTable);
			IReportCreator reportCreator = DataPageBuilder.CreateInstance(reportModel, dataManager);
//			reportCreator.SectionRendering += new EventHandler<SectionRenderEventArgs>(PushPrinting);
//			reportCreator.GroupHeaderRendering += new EventHandler<GroupHeaderEventArgs>(GroupHeaderRendering);
//			reportCreator.GroupFooterRendering += GroupFooterRendering;
//
//			reportCreator.RowRendering += new EventHandler<RowRenderEventArgs>(RowRendering);
			reportCreator.PageCreated += OnPageCreated;
			reportCreator.BuildExportList();
		}
		
		public void RunReport(ReportModel reportModel, System.Collections.IList dataSource, ReportParameters parameters)
		{
			if (reportModel == null) {
				throw new ArgumentNullException("reportModel");
			}
			if (dataSource == null) {
				throw new ArgumentNullException("dataSource");
			}
			ReportEngine.CheckForParameters(reportModel, parameters);
			IDataManager dataManager = DataManagerFactory.CreateDataManager(reportModel, dataSource);

			RunReport(reportModel, dataManager);
		}
		
		public void RunReport(ReportModel reportModel, IDataManager dataManager)
		{
			if (reportModel == null) {
				throw new ArgumentNullException("reportModel");
			}
			if (dataManager == null) {
				throw new ArgumentNullException("dataManager");
			}
//			ReportEngine.CheckForParameters(reportModel, parameters);
			IReportCreator reportCreator = DataPageBuilder.CreateInstance(reportModel, dataManager);
			reportCreator.PageCreated += OnPageCreated;
			reportCreator.BuildExportList();
		}
		
		
		private void RunFormSheet(ReportModel reportModel)
		{
			IReportCreator reportCreator = FormPageBuilder.CreateInstance(reportModel);
//			reportCreator.SectionRendering += new EventHandler<SectionRenderEventArgs>(PushPrinting);
			reportCreator.PageCreated += OnPageCreated;
			reportCreator.BuildExportList();
		}
		
		
		
		private void OnPageCreated (object sender,PageCreatedEventArgs e)
		{
			Pages.Add(e.SinglePage);
		}
	}
}