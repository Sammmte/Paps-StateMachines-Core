﻿using System;

namespace Paps.StateMachines
{
    public class EmptyStateMachineException : Exception
    {
        public EmptyStateMachineException()
        {

        }

        public EmptyStateMachineException(string message) : base(message)
        {

        }
    }
}


