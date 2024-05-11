using System.Collections;
using System.Collections.Generic;
using Codebase.Audio;
using UnityEngine;

namespace Codebase.Audio
{
    public partial class AudioRepository : ScriptableObject
    {
        public AudioClipReference[] playerFootSteps;
        public AudioClipReference playerShortAttack;
        public AudioClipReference playerLongAttack;
        public AudioClipReference playerJump;
        public AudioClipReference playerDead;
        public AudioClipReference playerDamageTaken;
        public AudioClipReference music;
        public AudioClipReference playerFallHit;
        public AudioClipReference healthReduce;
        public AudioClipReference[] ladderClimb;
        public AudioClipReference dash;
        public AudioClipReference sparkAbsorb;
    }
}


