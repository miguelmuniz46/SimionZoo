/*
	SimionZoo: A framework for online model-free Reinforcement Learning on continuous
	control problems

	Copyright (c) 2016 SimionSoft. https://github.com/simionsoft

	Permission is hereby granted, free of charge, to any person obtaining a copy
	of this software and associated documentation files (the "Software"), to deal
	in the Software without restriction, including without limitation the rights
	to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
	copies of the Software, and to permit persons to whom the Software is
	furnished to do so, subject to the following conditions:

	The above copyright notice and this permission notice shall be included in all
	copies or substantial portions of the Software.

	THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
	IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
	FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
	AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
	LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
	OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
	SOFTWARE.
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Windows.Forms;

using Caliburn.Micro;

using Herd.Files;

using Badger.ViewModels;
using Badger.Data;

namespace Badger
{
    public static class Files
    {
        public const string ExperimentBatchFilter = "*" + Extensions.ExperimentBatchExtension;
        public const string ProjectFilter = "*" + Extensions.ProjectExtension;
        public const string ExperimentFilter = "*" + Extensions.ExperimentExtension;
        public const string ReportFilter = "*" + Extensions.ReportExtension;

        public const string ExperimentFileTypeDescription = "Experiment";
        public const string ExperimentBatchDescription = "Batch";
        public const string ProjectDescription = "Project";
        public const string ReportDescription = "Report";
        public const string ExperimentBatchOrReportFileTypeDescription = "File";

        public const string ExperimentOrProjectFilter = ExperimentFilter + ";" + ProjectFilter;
        public const string ExperimentBatchOrReportFilter= ExperimentBatchFilter + ";" + ReportFilter;

        public delegate void LogFunction(string message);
        public delegate void LoadUpdateFunction();
        public delegate void ProgressUpdateFunction(double progress);


        //baseDirectory: directory where the batch file is located. Included to allow using paths
        //relative to the batch file, instead of relative to the RLSimion folder structure
        public delegate int XmlNodeFunction(XmlNode node, string baseDirectory,LoadUpdateFunction loadUpdateFunction);

        ///// <summary>
        ///// This function cleans the log files
        ///// </summary>
        ///// <param name="node">The node used in each call of the function</param>
        ///// <param name="baseDirectory">Base directory</param>
        ///// <param name="loadUpdateFunction">Callback function called after processing an experimental unit</param>
        ///// <returns></returns>
        //public static int CountFinishedExperimentalUnitsInBatch(XmlNode node, string baseDirectory
        //        , LoadUpdateFunction loadUpdateFunction)
        //{
        //    int finishedExperimentalUnits = 0;
        //    foreach (XmlNode child in node.ChildNodes)
        //    {
        //        if (child.Name == XMLTags.ExperimentalUnitNodeTag)
        //        {
        //            string pathToExpUnit = baseDirectory + child.Attributes[XMLTags.pathAttribute].Value;
        //            string pathToLogFile = Herd.Utils.GetLogFilePath(pathToExpUnit, false);
        //            if (File.Exists(pathToLogFile))
        //            {
        //                FileInfo fileInfo = new FileInfo(pathToLogFile);
        //                if (fileInfo.Length > 0)
        //                    finishedExperimentalUnits++;
        //            }
        //        }
        //        else
        //            finishedExperimentalUnits+= CountFinishedExperimentalUnitsInBatch(child, baseDirectory, loadUpdateFunction);
        //    }
        //    return finishedExperimentalUnits;
        //}

        ///// <summary>
        ///// This function cleans the log files
        ///// </summary>
        ///// <param name="node">The node used in each call of the function</param>
        ///// <param name="baseDirectory">Base directory</param>
        ///// <param name="loadUpdateFunction">Callback function called after processing an experimental unit</param>
        ///// <returns></returns>
        //public static int CleanExperimentalUnitLogFiles(XmlNode node, string baseDirectory
        //        , LoadUpdateFunction loadUpdateFunction)
        //{
        //    int numFilesDeleted = 0;
        //    foreach (XmlNode child in node.ChildNodes)
        //    {
        //        if (child.Name == XMLTags.ExperimentalUnitNodeTag)
        //        {
        //            string pathToExpUnit = baseDirectory + child.Attributes[XMLTags.pathAttribute].Value;
        //            string pathToLogFile = Herd.Utils.GetLogFilePath(pathToExpUnit, false);
        //            if (File.Exists(pathToLogFile))
        //            {
        //                File.Delete(pathToLogFile);
        //                numFilesDeleted++;
        //            }
        //            string pathToLogFileDesc = Herd.Utils.GetLogFilePath(pathToExpUnit, true);
        //            if (File.Exists(pathToLogFileDesc))
        //            {
        //                File.Delete(pathToLogFileDesc);
        //                numFilesDeleted++;
        //            }
        //        }
        //        else
        //            numFilesDeleted+= CleanExperimentalUnitLogFiles(child, baseDirectory, loadUpdateFunction);
        //    }
        //    return numFilesDeleted;
        //}
        ///// <summary>
        ///// This function can be passed as an argument to LoadExperimentBatch to quickly count the number
        ///// of experimental units in a batch
        ///// </summary>
        ///// <param name="node">The node used in each call of the function</param>
        ///// <param name="baseDirectory">Base directory</param>
        ///// <param name="loadUpdateFunction">Callback function called after processing an experimental unit</param>
        ///// <returns></returns>
        //public static int CountExperimentalUnitsInBatch(XmlNode node, string baseDirectory
        //        , LoadUpdateFunction loadUpdateFunction)
        //{
        //    int ExperimentalUnitCount = 0;
        //    foreach (XmlNode child in node.ChildNodes)
        //    {
        //        if (child.Name == XMLTags.ExperimentalUnitNodeTag)
        //            ExperimentalUnitCount++;
        //        else ExperimentalUnitCount += CountExperimentalUnitsInBatch(child, null, loadUpdateFunction);
        //    }
        //    return ExperimentalUnitCount;
        //}

        
        ///// <summary>
        /////     Load experiment batch file.
        ///// </summary>
        ///// <param name="perExperimentFunction"></param>
        ///// <param name="batchFilename"></param>
        //static public int LoadExperimentBatchFile(string batchFilename, LoadOptions loadOptions)
        //{
        //    LoggedExperimentBatch batch = new LoggedExperimentBatch(batchFilename, loadOptions);
        //    int retValue = 0;
        //    // Load the experiment batch in queue
        //    XmlDocument batchDoc = new XmlDocument();
        //    batchDoc.Load(batchFilename);
        //    XmlElement fileRoot = batchDoc.DocumentElement;

        //    if (fileRoot != null && fileRoot.Name == XMLTags.ExperimentBatchNodeTag)
        //    {
        //        foreach (XmlNode experiment in fileRoot.ChildNodes)
        //        {
        //            if (experiment.Name == XMLTags.ExperimentNodeTag)
        //                retValue+= perExperimentFunction(experiment, Herd.Utils.GetDirectory(batchFilename), onExperimentLoaded);
        //        }
        //    }
        //    else
        //    {
        //        CaliburnUtility.ShowWarningDialog("Malformed XML in experiment queue file. No badger node.", "ERROR");
        //        return retValue;
        //    }

        //    return retValue;
        //}

        /// <summary>
        ///     SAVE EXPERIMENT BATCH: the list of (possibly forked) experiments is saved a as set of experiments
        ///     without forks and a .exp-batch file in the root directory referencing them all and the forks/values
        ///     each one took.
        /// </summary>
        /// <param name="experiments"></param>
        /// <param name="batchFilename"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        public static int SaveExperimentBatchFile(BindableCollection<ExperimentViewModel> experiments,
            string batchFilename, LogFunction log, ProgressUpdateFunction progressUpdateFunction)
        {
            //set culture as invariant to write numbers as in english
            System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;

            int experimentalUnitsCount = 0;

            if (experiments.Count == 0)
                return -1;

            //Validate the experiments
            foreach (ExperimentViewModel experiment in experiments)
            {
                if (!experiment.Validate())
                {
                    Data.CaliburnUtility.ShowWarningDialog("The configuration couldn't be validated in " + experiment.Name
                        + ". Please check it", "VALIDATION ERROR");

                    return -1;
                }
                experimentalUnitsCount += experiment.getNumForkCombinations();
            }

            //Dialog window asking for an output name
            if (batchFilename == "")
            {
                //Save dialog -> returns the experiment batch file
                var sfd = SaveFileDialog(ExperimentBatchDescription, ExperimentBatchFilter);
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    batchFilename = sfd.FileName;
                }
                else
                {
                    log("Error saving the experiment queue");
                    return -1;
                }
            }
            string batchFileDir = batchFilename.Remove(batchFilename.LastIndexOf(Extensions.ExperimentBatchExtension));
            string batchFileName = Herd.Utils.GetFilename(batchFileDir);
            batchFileDir = Herd.Utils.GetRelativePathTo(Directory.GetCurrentDirectory(), batchFileDir);
            // Clean output directory if it exists
            if (Directory.Exists(batchFileDir))
            {
                try { Directory.Delete(batchFileDir, true); }
                catch
                {
                    Data.CaliburnUtility.ShowWarningDialog("It has not been possible to remove the directory: "
                        + batchFileDir + ". Make sure that it's not been using for other app.", "ERROR");
                    log("Error saving the experiment queue");
                    return -1;
                }
            }
            string fullBatchFileName = batchFileDir + Herd.Files.Extensions.ExperimentBatchExtension;

            progressUpdateFunction?.Invoke(0.0);
            int numExperimentalUnitsSaved = 0;

            using (StreamWriter batchFileWriter = new StreamWriter(fullBatchFileName))
            {
                // Write batch file header (i.e. open 'EXPERIMENT-BATCH' tag)
                batchFileWriter.WriteLine("<" + XMLTags.ExperimentBatchNodeTag + ">");

                foreach (ExperimentViewModel experimentViewModel in experiments)
                {
                    batchFileWriter.WriteLine("  <" + XMLTags.ExperimentNodeTag + " "
                        + XMLTags.nameAttribute + "=\"" + experimentViewModel.Name + "\">");

                    foreach (AppVersion appVersion in experimentViewModel.AppVersions)
                    {
                        batchFileWriter.WriteLine(appVersion.ToString());
                    }

                    // Save the fork hierarchy and values. This helps to generate reports easier
                    experimentViewModel.SaveToStream(batchFileWriter, SaveMode.ForkHierarchy, "  ");

                    int numCombinations = experimentViewModel.getNumForkCombinations();
                    for (int i = 0; i < numCombinations; i++)
                    {
                        // Save the combination of forks as a new experiment
                        string experimentName = experimentViewModel.setForkCombination(i);

                        string folderPath = batchFileDir + "/" + experimentName;
                        Directory.CreateDirectory(folderPath);
                        string filePath = folderPath + "/" + experimentName + Extensions.ExperimentExtension;
                        experimentViewModel.Save(filePath, SaveMode.AsExperimentalUnit);
                        string relativePathToExperimentalUnit = batchFileName + "/" + experimentName + "/" + experimentName + Extensions.ExperimentExtension;

                        // Save the experiment reference in the root batch file. Open 'EXPERIMENTAL-UNIT' tag
                        // with its corresponding attributes.
                        batchFileWriter.WriteLine("    <" + XMLTags.ExperimentalUnitNodeTag + " "
                            + XMLTags.nameAttribute + "=\"" + experimentName + "\" "
                            + XMLTags.pathAttribute + "=\"" + relativePathToExperimentalUnit + "\">");
                        // Write fork values in between
                        experimentViewModel.SaveToStream(batchFileWriter, SaveMode.ForkValues, "\t");
                        // Close 'EXPERIMENTAL-UNIT' tag
                        batchFileWriter.WriteLine("    </" + XMLTags.ExperimentalUnitNodeTag + ">");

                        numExperimentalUnitsSaved++;
                        progressUpdateFunction?.Invoke(numExperimentalUnitsSaved / (double)experimentalUnitsCount);
                    }

                    // Close 'EXPERIMENT' tag
                    batchFileWriter.WriteLine("  </" + XMLTags.ExperimentNodeTag + ">");
                }
                // Write batch file footer (i.e. close 'EXPERIMENT-BATCH' tag)
                batchFileWriter.WriteLine("</" + XMLTags.ExperimentBatchNodeTag + ">");
                log("Succesfully saved " + experiments.Count + " experiments");
            }

            return experimentalUnitsCount;
        }


        //BADGER project: LOAD
        static public void LoadExperiments(ref BindableCollection<ExperimentViewModel> experiments,
            Dictionary<string, string> appDefinitions, LogFunction log, string filename= null)
        {
            if (filename == null)
            {
                bool isOpen = OpenFileDialog(ref filename, ProjectDescription, ProjectFilter);
                if (!isOpen)
                    return;
            }

            XmlDocument badgerDoc = new XmlDocument();
            badgerDoc.Load(filename);
            XmlElement fileRoot = badgerDoc.DocumentElement;
            if (fileRoot.Name != XMLTags.BadgerNodeTag)
            {
                Data.CaliburnUtility.ShowWarningDialog("Malformed XML in experiment badger project file.", "ERROR");
                log("ERROR: malformed XML in experiment badger project file.");
                return;
            }

            XmlNode configNode;
            foreach (XmlNode experiment in fileRoot.ChildNodes)
            {
                if (experiment.Name == XMLTags.ExperimentNodeTag && experiment.ChildNodes.Count > 0)
                {
                    configNode = experiment.FirstChild;
                    experiments.Add(new ExperimentViewModel(appDefinitions[configNode.Name],
                        configNode, experiment.Attributes[XMLTags.nameAttribute].Value));
                }
                else
                {
                    Data.CaliburnUtility.ShowWarningDialog("Malformed XML in experiment queue file.", "ERROR");
                    log("ERROR: malformed XML in experiment queue file.");
                }
            }
        }

        /// <summary>
        ///     Save multiple experiments as a Badger project. Useful when we need to load a bunch
        ///     of experiments later.
        /// </summary>
        /// <param name="experiments"></param>
        static public string SaveExperiments(BindableCollection<ExperimentViewModel> experiments, string outputFile = null)
        {
            //set culture as invariant to write numbers as in english
            System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;

            foreach (ExperimentViewModel experiment in experiments)
            {
                if (!experiment.Validate())
                {
                    Data.CaliburnUtility.ShowWarningDialog("The configuration couldn't be validated in " + experiment.Name
                        + ". Please check it", "VALIDATION ERROR");
                    return null;
                }
            }

            if (outputFile == null)
            {
                var sfd = SaveFileDialog(ProjectDescription, ProjectFilter);
                if (sfd.ShowDialog() == DialogResult.OK)
                    outputFile = sfd.FileName;
            }

            if (outputFile != null) //either because a name was provided when the method was called or because the user selected one from the dialog
            {
                string leftSpace;
                using (StreamWriter writer = new StreamWriter(outputFile))
                {
                    writer.WriteLine("<" + XMLTags.BadgerNodeTag + " " + XMLTags.versionAttribute
                        + "=\"" + XMLTags.BadgerProjectConfigVersion + "\">");
                    leftSpace = "  ";

                    foreach (ExperimentViewModel experiment in experiments)
                    {
                        writer.WriteLine(leftSpace + "<" + XMLTags.ExperimentNodeTag
                            + " Name=\"" + experiment.Name + "\">");
                        experiment.SaveToStream(writer, SaveMode.AsExperiment, leftSpace + "  ");
                        writer.WriteLine(leftSpace + "</" + XMLTags.ExperimentNodeTag + ">");
                    }

                    writer.WriteLine("</" + XMLTags.BadgerNodeTag + ">");
                }
            }
            return outputFile;
        }

        //EXPERIMENT file: LOAD
        /// <summary>
        /// 
        /// </summary>
        /// <param name="appDefinitions"></param>
        /// <returns></returns>
        static public ExperimentViewModel LoadExperiment(Dictionary<string, string> appDefinitions, string filename= null)
        {
            //set culture as invariant to write numbers as in english
            System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;

            if (filename == null)
            {
                bool isOpen = OpenFileDialog(ref filename, ExperimentFileTypeDescription, ExperimentFilter);
                if (!isOpen)
                    return null;
            }

            // Open the config file to retrive the app's name before loading it
            XmlDocument configDocument = new XmlDocument();
            configDocument.Load(filename);
            XmlNode rootNode = configDocument.LastChild;
            ExperimentViewModel newExperiment = new ExperimentViewModel(appDefinitions[rootNode.Name], filename);
            return newExperiment;
        }


        static public string SaveExperiment(ExperimentViewModel experiment)
        {
            //set culture as invariant to write numbers as in english
            System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;

            if (!experiment.Validate())
            {
                Data.CaliburnUtility.ShowWarningDialog("The configuration couldn't be validated in " + experiment.Name
                    + ". Please check it", "VALIDATION ERROR");
                return null;
            }

            var sfd = SaveFileDialog(ExperimentFileTypeDescription, ExperimentFilter);
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                experiment.Save(sfd.FileName, SaveMode.AsExperiment);
                return sfd.FileName;
            }

            return null;
        }


        static string globalOutputDirectory = null;
        static string globalInputDirectory = null;

        public static object Utils { get; internal set; }

        /// <summary>
        ///     Show a dialog used to save a file with an specific extension.
        /// </summary>
        /// <param name="description">The description of the file type</param>
        /// <param name="extension">The extension of the file</param>
        /// <returns></returns>
        public static SaveFileDialog SaveFileDialog(string description, string filter, string suggestedFileName= null)
        {
            SaveFileDialog sfd = new SaveFileDialog();

            sfd.Filter = description + "|" + filter;
            sfd.SupportMultiDottedExtensions = true;

            if (globalInputDirectory == null)
            {
                string initialDirectory = Path.Combine(Directory.GetCurrentDirectory(), Folders.experimentRelativeDir);

                if (!Directory.Exists(initialDirectory))
                    Directory.CreateDirectory(initialDirectory);

                sfd.InitialDirectory = Path.GetFullPath(initialDirectory);
            }
            else
                sfd.InitialDirectory = globalInputDirectory;

            if (suggestedFileName != null)
            {
                globalInputDirectory = Herd.Utils.GetDirectory(sfd.FileName);
                sfd.FileName = suggestedFileName;
            }
            return sfd;
        }

        /// <summary>
        ///     Show a dialog used to select a directory where files are to be saved. If something goes wrong, null is returned
        /// </summary>
        /// <param name="description">The description of the file type</param>
        /// <param name="extension">The extension of the file</param>
        /// <returns></returns>
        public static string SelectOutputDirectoryDialog(string initialDirectory= null)
        {
            
            var folderBrowserDialog = new FolderBrowserDialog();

            folderBrowserDialog.RootFolder = Environment.SpecialFolder.Desktop;

            if (globalOutputDirectory == null)
                folderBrowserDialog.SelectedPath = initialDirectory;
            else
                folderBrowserDialog.SelectedPath = globalOutputDirectory;

            // Show the FolderBrowserDialog.
            DialogResult result = folderBrowserDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                globalOutputDirectory = folderBrowserDialog.SelectedPath;
                return folderBrowserDialog.SelectedPath;
            }
            return null;
        }

        /// <summary>
        ///     Open a file that fulfills the requirements passed as parameters.
        /// </summary>
        /// <param name="fileName">The name of the file</param>
        /// <param name="description">Description of the file type</param>
        /// <param name="extension">The extension of the file type</param>
        /// <returns>Wheter the file was successfully open or not</returns>
        public static bool OpenFileDialog(ref string fileName, string description, string extension)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = description + "|" + extension;

            if (globalInputDirectory == null)
                ofd.InitialDirectory = Path.Combine(Path.GetDirectoryName(Directory.GetCurrentDirectory()), Folders.experimentDir);
            else
                ofd.InitialDirectory = globalInputDirectory;

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                globalInputDirectory = Herd.Utils.GetDirectory(ofd.FileName);
                fileName = ofd.FileName;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Opens a dialog window to select files of different types
        /// </summary>
        /// <param name="extensions">Extensions (separated by a semicolon</param>
        /// <param name="descriptions">Description for each extension (separated by a semicolon)</param>
        /// <param name="multiSelect">Allow the user to select more than one file?</param>
        /// <returns>A list of the selected files</returns>
        public static List<string> OpenFileDialogMultipleFiles(string extensions, string descriptions, bool multiSelect= true)
        {
            List<string> filenames = new List<string>();
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = multiSelect;
            ofd.SupportMultiDottedExtensions = true;
            ofd.Filter = extensions + "|" + descriptions;

            if (globalInputDirectory == null)
            {
                ofd.InitialDirectory = Path.Combine(Path.GetDirectoryName(Directory.GetCurrentDirectory())
                    , Folders.experimentDir);
            }
            else
                ofd.InitialDirectory = globalInputDirectory;

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                foreach (string filename in ofd.FileNames)
                    filenames.Add(filename);
            }

            return filenames;
        }

        /// <summary>
        /// This method saves a set of queries on a experiment batch and their results to some output folder
        /// </summary>
        /// <param name="absBatchFilename">The absolute path to the experiment batch to which the query belongs</param>
        /// <param name="logQueryResults">The set of queries to be saved</param>
        /// <param name="absOutputFolder">Absolute path to the output folder, the name of the output report is taken from the input batch file</param>
        static public void SaveReport(string absBatchFilename, BindableCollection<LogQueryResultViewModel> logQueryResults, string absOutputFolder)
        {
            //save the queries and the report (a set of log query results referencing the queries and the experiment batch)
            string nakedBatchFilename = Herd.Utils.RemoveDirectories(Herd.Utils.RemoveExtension(absBatchFilename, 2));
            string reportFilename = absOutputFolder + "/" + nakedBatchFilename + Herd.Files.Extensions.ReportExtension;
            using (TextWriter writer = File.CreateText(reportFilename))
            {
                //open the root node in the report
                writer.WriteLine("<" + XMLTags.ReportNodeTag + " " + XMLTags.BatchFilenameAttr + "=\"" + absBatchFilename + "\">");

                foreach (LogQueryResultViewModel logQueryResult in logQueryResults)
                {
                    string nakedLogQueryName = Herd.Utils.RemoveSpecialCharacters(logQueryResult.Name);
                    //Save the reports, each one in a different subfolder
                    string absQueryOutputFolder = absOutputFolder + "/" + nakedLogQueryName;
                    string relOutputFolder = nakedLogQueryName;

                    if (!Directory.Exists(absQueryOutputFolder))
                        Directory.CreateDirectory(absQueryOutputFolder);

                    //Save serializable data
                    string relLogQueryResultFilename = nakedLogQueryName + "/" + nakedLogQueryName + ".xml";
                    string absLogQueryResultFilename = absOutputFolder + "/" + relLogQueryResultFilename;
                    Serialiazer.WriteObject(absLogQueryResultFilename, logQueryResult);

                    //Save non-serializable data
                    Dictionary<string,string> additionalOutputFiles= logQueryResult.ExportNonSerializable(absQueryOutputFolder);

                    //the reference to the query and non-serializable data files
                    writer.WriteLine("  <" + XMLTags.QueryNodeTag + " " + XMLTags.nameAttribute + "=\"" + relLogQueryResultFilename + "\">");

                    foreach(string file in additionalOutputFiles.Keys)
                    {
                        writer.WriteLine("    <" + additionalOutputFiles[file] + ">" + file + "</" + additionalOutputFiles[file] + ">");
                    }
                    writer.WriteLine("  </" + XMLTags.QueryNodeTag + ">");
                }

                //close the root node in the report
                writer.WriteLine("</" + XMLTags.ReportNodeTag + ">");
            }
        }

        static public int LoadReport(string reportFilename, out string batchFilename, out BindableCollection<LogQueryResultViewModel> logQueryResults)
        {
            logQueryResults = new BindableCollection<LogQueryResultViewModel>();
            XmlDocument reportDoc= new XmlDocument();
            reportDoc.Load(reportFilename);
            XmlNode root= reportDoc.FirstChild;
            batchFilename = null;
            string inputBaseFolder = Herd.Utils.GetDirectory(reportFilename);

            if (root.Name==XMLTags.ReportNodeTag)
            {
                batchFilename = root.Attributes[XMLTags.BatchFilenameAttr].Value;
                
                foreach(XmlNode child in root.ChildNodes)
                {
                    if (child.Name==XMLTags.QueryNodeTag)
                    {
                        LogQueryResultViewModel logQueryResult;
                        string queryFilename = inputBaseFolder + child.Attributes[XMLTags.nameAttribute].Value;
                        
                        if (File.Exists(queryFilename))
                        {
                            logQueryResult = Serialiazer.ReadObject<LogQueryResultViewModel>(queryFilename);
                            logQueryResult.ImportNonSerializable(Herd.Utils.GetDirectory(queryFilename));
                            logQueryResult.IsNotifying = true;
                            logQueryResults.Add(logQueryResult);
                        }
                    }
                }
            }

            return logQueryResults.Count;
        }
    }
}

