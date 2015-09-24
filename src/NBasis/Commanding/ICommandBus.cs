﻿using System.Collections.Generic;
using System.Threading.Tasks;

namespace NBasis.Commanding
{
    public interface ICommandBus
    {
        /// <summary>
        /// Send command(s) to the bus
        /// </summary>
        /// <returns></returns>
        Task Send(IEnumerable<Envelope<ICommand>> commands);

        /// <summary>
        /// Send a command and wait around for the result
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="command"></param>
        /// <returns></returns>
        Task<TResult> Ask<TResult>(Envelope<ICommand> command);
    }
}
