// ***********************************************************************
// Assembly         : 
// Author           : Guilherme Branco Stracini
// Created          : 07-23-2020
//
// Last Modified By : Guilherme Branco Stracini
// Last Modified On : 07-23-2020
// ***********************************************************************
// <copyright file="OrderServices.cs" company="Guilherme Branco Stracini ME">
//     Copyright (c) Guilherme Branco Stracini ME. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using RpcWorker.Domain;

namespace RpcWorker.Services
{
    /// <summary>
    /// Class OrderServices. This class cannot be inherited.
    /// </summary>
    public sealed class OrderServices
    {
        /// <summary>
        /// Called when [store].
        /// </summary>
        /// <param name="amount">The amount.</param>
        /// <returns>OrderStatus.</returns>
        public static OrderStatus OnStore(decimal amount)
        {
            return (amount < 0 || amount > 10000) ? OrderStatus.Declined : OrderStatus.Approved;
        }
    }
}
