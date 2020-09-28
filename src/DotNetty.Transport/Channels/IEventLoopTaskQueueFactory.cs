﻿/*
 * Copyright 2012 The Netty Project
 *
 * The Netty Project licenses this file to you under the Apache License,
 * version 2.0 (the "License"); you may not use this file except in compliance
 * with the License. You may obtain a copy of the License at:
 *
 *   http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
 * License for the specific language governing permissions and limitations
 * under the License.
 *
 * Copyright (c) 2020 The Dotnetty-Span-Fork Project (cuteant@outlook.com) All rights reserved.
 *
 *   https://github.com/cuteant/dotnetty-span-fork
 *
 * Licensed under the MIT license. See LICENSE file in the project root for full license information.
 */

namespace DotNetty.Transport.Channels
{
    using DotNetty.Common.Concurrency;
    using DotNetty.Common.Internal;

    /// <summary>
    /// Factory used to create <see cref="IQueue{T}"/> instances that will be used to store tasks for an <see cref="IEventLoop"/>.
    /// 
    /// <para>Generally speaking the returned <see cref="IQueue{T}"/> MUST be thread-safe and depending on the <see cref="IEventLoop"/>
    /// implementation must be of type <see cref="IBlockingQueue{T}"/>.</para>
    /// </summary>
    public interface IEventLoopTaskQueueFactory
    {
        /// <summary>
        /// Returns a new <see cref="IQueue{T}"/> to use.
        /// </summary>
        /// <param name="maxCapacity">the maximum amount of elements that can be stored in the <see cref="IQueue{T}"/> at a given point
        /// in time.</param>
        /// <returns>the new queue.</returns>
        IQueue<IRunnable> NewTaskQueue(int maxCapacity);
    }
}