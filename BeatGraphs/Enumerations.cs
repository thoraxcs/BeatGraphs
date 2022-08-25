﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeatGraphs
{
    // Loop resolution methods
    public enum Method
    {
        Standard,
        Iterative,
        Weighted
    }

    // Log levels for text output
    public enum LogLevel
    {
        verbose,
        info,
        warning,
        error
    }

    // File path to use when working with directories
    public enum BasePath
    {
        settings,
        file
    }
}