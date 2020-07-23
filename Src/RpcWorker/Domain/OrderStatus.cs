// ***********************************************************************
// Assembly         : RpcWorker
// Author           : Guilherme Branco Stracini
// Created          : 07-23-2020
//
// Last Modified By : Guilherme Branco Stracini
// Last Modified On : 07-23-2020
// ***********************************************************************
// <copyright file="OrderStatus.cs" company="Guilherme Branco Stracini ME">
//     Copyright (c) Guilherme Branco Stracini ME. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
namespace RpcWorker.Domain
{
    /// <summary>
    /// Enum OrderStatus
    /// </summary>
    public enum OrderStatus
    {
        /// <summary>
        /// The processing
        /// </summary>
        Processing = 0,

        /// <summary>
        /// The approved
        /// </summary>
        Approved,

        /// <summary>
        /// The declined
        /// </summary>
        Declined
    }
}
