﻿using System;

namespace Store
{
    public class FileHandleEventArgs : EventArgs
    {
        private readonly FileHandle handle;

        public FileHandleEventArgs(FileHandle handle)
        {
            this.handle = handle;
        }

        public FileHandle Handle
        {
            get { return handle; }
        }
    }
}
