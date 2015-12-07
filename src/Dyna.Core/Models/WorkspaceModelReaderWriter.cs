﻿using System;

namespace Dyna.Core.Models
{
    public sealed class WorkspaceModelReaderWriter : IWorkspaceModelReaderWriter
    {
        private readonly IWorkspaceModelReader reader;
        private readonly IWorkspaceModelReaderWriter writer;

        public WorkspaceModelReaderWriter(IWorkspaceModelReader theReader,
                                          IWorkspaceModelReaderWriter theWriter)
        {
            if (theReader == null)
                throw new ArgumentNullException("theReader");
            if (theWriter == null)
                throw new ArgumentNullException("theWriter");
            this.reader = theReader;
            this.writer = theWriter;
        }

        /// <summary>
        /// Read a workspace model from a file.
        /// </summary>
        /// <returns>Workspace model.</returns>
        public WorkspaceModel Read(string filename)
        {
            return this.reader.Read(filename);
        }

        /// <summary>
        /// Write a workspace model to a file.
        /// </summary>
        /// <param name="filename">File Path.</param>
        /// <param name="theWorkspace">Workspace model.</param>
        public void Write(string filename, WorkspaceModel theWorkspace)
        {
            this.writer.Write(filename, theWorkspace);
        }
    }
}
