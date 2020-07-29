// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace MessagePack.Internal
{
    public static class DateTimeConstants
    {
        public const long BclSecondsAtUnixEpoch = 62135596800;
        public const int NanosecondsPerTick = 100;
        public static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    }
}
