using System;
using PaveEcsGame.UniversalDelegates;

public interface ISupportForEachWithUniversalDelegate
{
}


namespace PaveEcsGame.UniversalDelegates
{
    public delegate void R<T0>(ref T0 t0);

    public delegate void I<T0>(in T0 t0);

    public delegate void V<T0>(T0 t0);

    public delegate void RI<T0, T1>(ref T0 t0, in T1 t1);

    public delegate void RR<T0, T1>(ref T0 t0, ref T1 t1);

    public delegate void II<T0, T1>(in T0 t0, in T1 t1);

    public delegate void VI<T0, T1>(T0 t0, in T1 t1);

    public delegate void VR<T0, T1>(T0 t0, ref T1 t1);

    public delegate void VV<T0, T1>(T0 t0, T1 t1);

    public delegate void RII<T0, T1, T2>(ref T0 t0, in T1 t1, in T2 t2);

    public delegate void RRI<T0, T1, T2>(ref T0 t0, ref T1 t1, in T2 t2);

    public delegate void RRR<T0, T1, T2>(ref T0 t0, ref T1 t1, ref T2 t2);

    public delegate void III<T0, T1, T2>(in T0 t0, in T1 t1, in T2 t2);

    public delegate void VII<T0, T1, T2>(T0 t0, in T1 t1, in T2 t2);

    public delegate void VRI<T0, T1, T2>(T0 t0, ref T1 t1, in T2 t2);

    public delegate void VRR<T0, T1, T2>(T0 t0, ref T1 t1, ref T2 t2);

    public delegate void VVI<T0, T1, T2>(T0 t0, T1 t1, in T2 t2);

    public delegate void VVR<T0, T1, T2>(T0 t0, T1 t1, ref T2 t2);

    public delegate void VVV<T0, T1, T2>(T0 t0, T1 t1, T2 t2);

    public delegate void RIII<T0, T1, T2, T3>(ref T0 t0, in T1 t1, in T2 t2, in T3 t3);

    public delegate void RRII<T0, T1, T2, T3>(ref T0 t0, ref T1 t1, in T2 t2, in T3 t3);

    public delegate void RRRI<T0, T1, T2, T3>(ref T0 t0, ref T1 t1, ref T2 t2, in T3 t3);

    public delegate void RRRR<T0, T1, T2, T3>(ref T0 t0, ref T1 t1, ref T2 t2, ref T3 t3);

    public delegate void IIII<T0, T1, T2, T3>(in T0 t0, in T1 t1, in T2 t2, in T3 t3);

    public delegate void VIII<T0, T1, T2, T3>(T0 t0, in T1 t1, in T2 t2, in T3 t3);

    public delegate void VRII<T0, T1, T2, T3>(T0 t0, ref T1 t1, in T2 t2, in T3 t3);

    public delegate void VRRI<T0, T1, T2, T3>(T0 t0, ref T1 t1, ref T2 t2, in T3 t3);

    public delegate void VRRR<T0, T1, T2, T3>(T0 t0, ref T1 t1, ref T2 t2, ref T3 t3);

    public delegate void VVII<T0, T1, T2, T3>(T0 t0, T1 t1, in T2 t2, in T3 t3);

    public delegate void VVRI<T0, T1, T2, T3>(T0 t0, T1 t1, ref T2 t2, in T3 t3);

    public delegate void VVRR<T0, T1, T2, T3>(T0 t0, T1 t1, ref T2 t2, ref T3 t3);

    public delegate void VVVI<T0, T1, T2, T3>(T0 t0, T1 t1, T2 t2, in T3 t3);

    public delegate void VVVR<T0, T1, T2, T3>(T0 t0, T1 t1, T2 t2, ref T3 t3);

    public delegate void VVVV<T0, T1, T2, T3>(T0 t0, T1 t1, T2 t2, T3 t3);

    public delegate void RIIII<T0, T1, T2, T3, T4>(ref T0 t0, in T1 t1, in T2 t2, in T3 t3, in T4 t4);

    public delegate void RRIII<T0, T1, T2, T3, T4>(ref T0 t0, ref T1 t1, in T2 t2, in T3 t3, in T4 t4);

    public delegate void RRRII<T0, T1, T2, T3, T4>(ref T0 t0, ref T1 t1, ref T2 t2, in T3 t3, in T4 t4);

    public delegate void RRRRI<T0, T1, T2, T3, T4>(ref T0 t0, ref T1 t1, ref T2 t2, ref T3 t3, in T4 t4);

    public delegate void RRRRR<T0, T1, T2, T3, T4>(ref T0 t0, ref T1 t1, ref T2 t2, ref T3 t3, ref T4 t4);

    public delegate void IIIII<T0, T1, T2, T3, T4>(in T0 t0, in T1 t1, in T2 t2, in T3 t3, in T4 t4);

    public delegate void VIIII<T0, T1, T2, T3, T4>(T0 t0, in T1 t1, in T2 t2, in T3 t3, in T4 t4);

    public delegate void VRIII<T0, T1, T2, T3, T4>(T0 t0, ref T1 t1, in T2 t2, in T3 t3, in T4 t4);

    public delegate void VRRII<T0, T1, T2, T3, T4>(T0 t0, ref T1 t1, ref T2 t2, in T3 t3, in T4 t4);

    public delegate void VRRRI<T0, T1, T2, T3, T4>(T0 t0, ref T1 t1, ref T2 t2, ref T3 t3, in T4 t4);

    public delegate void VRRRR<T0, T1, T2, T3, T4>(T0 t0, ref T1 t1, ref T2 t2, ref T3 t3, ref T4 t4);

    public delegate void VVIII<T0, T1, T2, T3, T4>(T0 t0, T1 t1, in T2 t2, in T3 t3, in T4 t4);

    public delegate void VVRII<T0, T1, T2, T3, T4>(T0 t0, T1 t1, ref T2 t2, in T3 t3, in T4 t4);

    public delegate void VVRRI<T0, T1, T2, T3, T4>(T0 t0, T1 t1, ref T2 t2, ref T3 t3, in T4 t4);

    public delegate void VVRRR<T0, T1, T2, T3, T4>(T0 t0, T1 t1, ref T2 t2, ref T3 t3, ref T4 t4);

    public delegate void VVVII<T0, T1, T2, T3, T4>(T0 t0, T1 t1, T2 t2, in T3 t3, in T4 t4);

    public delegate void VVVRI<T0, T1, T2, T3, T4>(T0 t0, T1 t1, T2 t2, ref T3 t3, in T4 t4);

    public delegate void VVVRR<T0, T1, T2, T3, T4>(T0 t0, T1 t1, T2 t2, ref T3 t3, ref T4 t4);

    public delegate void VVVVI<T0, T1, T2, T3, T4>(T0 t0, T1 t1, T2 t2, T3 t3, in T4 t4);

    public delegate void VVVVR<T0, T1, T2, T3, T4>(T0 t0, T1 t1, T2 t2, T3 t3, ref T4 t4);

    public delegate void VVVVV<T0, T1, T2, T3, T4>(T0 t0, T1 t1, T2 t2, T3 t3, T4 t4);

    public delegate void RIIIII<T0, T1, T2, T3, T4, T5>(ref T0 t0, in T1 t1, in T2 t2, in T3 t3, in T4 t4, in T5 t5);

    public delegate void RRIIII<T0, T1, T2, T3, T4, T5>(ref T0 t0, ref T1 t1, in T2 t2, in T3 t3, in T4 t4, in T5 t5);

    public delegate void RRRIII<T0, T1, T2, T3, T4, T5>(ref T0 t0, ref T1 t1, ref T2 t2, in T3 t3, in T4 t4, in T5 t5);

    public delegate void RRRRII<T0, T1, T2, T3, T4, T5>(ref T0 t0, ref T1 t1, ref T2 t2, ref T3 t3, in T4 t4, in T5 t5);

    public delegate void
        RRRRRI<T0, T1, T2, T3, T4, T5>(ref T0 t0, ref T1 t1, ref T2 t2, ref T3 t3, ref T4 t4, in T5 t5);

    public delegate void RRRRRR<T0, T1, T2, T3, T4, T5>(ref T0 t0, ref T1 t1, ref T2 t2, ref T3 t3, ref T4 t4,
        ref T5 t5);

    public delegate void IIIIII<T0, T1, T2, T3, T4, T5>(in T0 t0, in T1 t1, in T2 t2, in T3 t3, in T4 t4, in T5 t5);

    public delegate void VIIIII<T0, T1, T2, T3, T4, T5>(T0 t0, in T1 t1, in T2 t2, in T3 t3, in T4 t4, in T5 t5);

    public delegate void VRIIII<T0, T1, T2, T3, T4, T5>(T0 t0, ref T1 t1, in T2 t2, in T3 t3, in T4 t4, in T5 t5);

    public delegate void VRRIII<T0, T1, T2, T3, T4, T5>(T0 t0, ref T1 t1, ref T2 t2, in T3 t3, in T4 t4, in T5 t5);

    public delegate void VRRRII<T0, T1, T2, T3, T4, T5>(T0 t0, ref T1 t1, ref T2 t2, ref T3 t3, in T4 t4, in T5 t5);

    public delegate void VRRRRI<T0, T1, T2, T3, T4, T5>(T0 t0, ref T1 t1, ref T2 t2, ref T3 t3, ref T4 t4, in T5 t5);

    public delegate void VRRRRR<T0, T1, T2, T3, T4, T5>(T0 t0, ref T1 t1, ref T2 t2, ref T3 t3, ref T4 t4, ref T5 t5);

    public delegate void VVIIII<T0, T1, T2, T3, T4, T5>(T0 t0, T1 t1, in T2 t2, in T3 t3, in T4 t4, in T5 t5);

    public delegate void VVRIII<T0, T1, T2, T3, T4, T5>(T0 t0, T1 t1, ref T2 t2, in T3 t3, in T4 t4, in T5 t5);

    public delegate void VVRRII<T0, T1, T2, T3, T4, T5>(T0 t0, T1 t1, ref T2 t2, ref T3 t3, in T4 t4, in T5 t5);

    public delegate void VVRRRI<T0, T1, T2, T3, T4, T5>(T0 t0, T1 t1, ref T2 t2, ref T3 t3, ref T4 t4, in T5 t5);

    public delegate void VVRRRR<T0, T1, T2, T3, T4, T5>(T0 t0, T1 t1, ref T2 t2, ref T3 t3, ref T4 t4, ref T5 t5);

    public delegate void VVVIII<T0, T1, T2, T3, T4, T5>(T0 t0, T1 t1, T2 t2, in T3 t3, in T4 t4, in T5 t5);

    public delegate void VVVRII<T0, T1, T2, T3, T4, T5>(T0 t0, T1 t1, T2 t2, ref T3 t3, in T4 t4, in T5 t5);

    public delegate void VVVRRI<T0, T1, T2, T3, T4, T5>(T0 t0, T1 t1, T2 t2, ref T3 t3, ref T4 t4, in T5 t5);

    public delegate void VVVRRR<T0, T1, T2, T3, T4, T5>(T0 t0, T1 t1, T2 t2, ref T3 t3, ref T4 t4, ref T5 t5);

    public delegate void VVVVII<T0, T1, T2, T3, T4, T5>(T0 t0, T1 t1, T2 t2, T3 t3, in T4 t4, in T5 t5);

    public delegate void VVVVRI<T0, T1, T2, T3, T4, T5>(T0 t0, T1 t1, T2 t2, T3 t3, ref T4 t4, in T5 t5);

    public delegate void VVVVRR<T0, T1, T2, T3, T4, T5>(T0 t0, T1 t1, T2 t2, T3 t3, ref T4 t4, ref T5 t5);

    public delegate void VVVVVI<T0, T1, T2, T3, T4, T5>(T0 t0, T1 t1, T2 t2, T3 t3, T4 t4, in T5 t5);

    public delegate void VVVVVR<T0, T1, T2, T3, T4, T5>(T0 t0, T1 t1, T2 t2, T3 t3, T4 t4, ref T5 t5);

    public delegate void VVVVVV<T0, T1, T2, T3, T4, T5>(T0 t0, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5);

    public delegate void RIIIIII<T0, T1, T2, T3, T4, T5, T6>(ref T0 t0, in T1 t1, in T2 t2, in T3 t3, in T4 t4,
        in T5 t5, in T6 t6);

    public delegate void RRIIIII<T0, T1, T2, T3, T4, T5, T6>(ref T0 t0, ref T1 t1, in T2 t2, in T3 t3, in T4 t4,
        in T5 t5, in T6 t6);

    public delegate void RRRIIII<T0, T1, T2, T3, T4, T5, T6>(ref T0 t0, ref T1 t1, ref T2 t2, in T3 t3, in T4 t4,
        in T5 t5, in T6 t6);

    public delegate void RRRRIII<T0, T1, T2, T3, T4, T5, T6>(ref T0 t0, ref T1 t1, ref T2 t2, ref T3 t3, in T4 t4,
        in T5 t5, in T6 t6);

    public delegate void RRRRRII<T0, T1, T2, T3, T4, T5, T6>(ref T0 t0, ref T1 t1, ref T2 t2, ref T3 t3, ref T4 t4,
        in T5 t5, in T6 t6);

    public delegate void RRRRRRI<T0, T1, T2, T3, T4, T5, T6>(ref T0 t0, ref T1 t1, ref T2 t2, ref T3 t3, ref T4 t4,
        ref T5 t5, in T6 t6);

    public delegate void RRRRRRR<T0, T1, T2, T3, T4, T5, T6>(ref T0 t0, ref T1 t1, ref T2 t2, ref T3 t3, ref T4 t4,
        ref T5 t5, ref T6 t6);

    public delegate void IIIIIII<T0, T1, T2, T3, T4, T5, T6>(in T0 t0, in T1 t1, in T2 t2, in T3 t3, in T4 t4, in T5 t5,
        in T6 t6);

    public delegate void VIIIIII<T0, T1, T2, T3, T4, T5, T6>(T0 t0, in T1 t1, in T2 t2, in T3 t3, in T4 t4, in T5 t5,
        in T6 t6);

    public delegate void VRIIIII<T0, T1, T2, T3, T4, T5, T6>(T0 t0, ref T1 t1, in T2 t2, in T3 t3, in T4 t4, in T5 t5,
        in T6 t6);

    public delegate void VRRIIII<T0, T1, T2, T3, T4, T5, T6>(T0 t0, ref T1 t1, ref T2 t2, in T3 t3, in T4 t4, in T5 t5,
        in T6 t6);

    public delegate void VRRRIII<T0, T1, T2, T3, T4, T5, T6>(T0 t0, ref T1 t1, ref T2 t2, ref T3 t3, in T4 t4, in T5 t5,
        in T6 t6);

    public delegate void VRRRRII<T0, T1, T2, T3, T4, T5, T6>(T0 t0, ref T1 t1, ref T2 t2, ref T3 t3, ref T4 t4,
        in T5 t5, in T6 t6);

    public delegate void VRRRRRI<T0, T1, T2, T3, T4, T5, T6>(T0 t0, ref T1 t1, ref T2 t2, ref T3 t3, ref T4 t4,
        ref T5 t5, in T6 t6);

    public delegate void VRRRRRR<T0, T1, T2, T3, T4, T5, T6>(T0 t0, ref T1 t1, ref T2 t2, ref T3 t3, ref T4 t4,
        ref T5 t5, ref T6 t6);

    public delegate void VVIIIII<T0, T1, T2, T3, T4, T5, T6>(T0 t0, T1 t1, in T2 t2, in T3 t3, in T4 t4, in T5 t5,
        in T6 t6);

    public delegate void VVRIIII<T0, T1, T2, T3, T4, T5, T6>(T0 t0, T1 t1, ref T2 t2, in T3 t3, in T4 t4, in T5 t5,
        in T6 t6);

    public delegate void VVRRIII<T0, T1, T2, T3, T4, T5, T6>(T0 t0, T1 t1, ref T2 t2, ref T3 t3, in T4 t4, in T5 t5,
        in T6 t6);

    public delegate void VVRRRII<T0, T1, T2, T3, T4, T5, T6>(T0 t0, T1 t1, ref T2 t2, ref T3 t3, ref T4 t4, in T5 t5,
        in T6 t6);

    public delegate void VVRRRRI<T0, T1, T2, T3, T4, T5, T6>(T0 t0, T1 t1, ref T2 t2, ref T3 t3, ref T4 t4, ref T5 t5,
        in T6 t6);

    public delegate void VVRRRRR<T0, T1, T2, T3, T4, T5, T6>(T0 t0, T1 t1, ref T2 t2, ref T3 t3, ref T4 t4, ref T5 t5,
        ref T6 t6);

    public delegate void VVVIIII<T0, T1, T2, T3, T4, T5, T6>(T0 t0, T1 t1, T2 t2, in T3 t3, in T4 t4, in T5 t5,
        in T6 t6);

    public delegate void VVVRIII<T0, T1, T2, T3, T4, T5, T6>(T0 t0, T1 t1, T2 t2, ref T3 t3, in T4 t4, in T5 t5,
        in T6 t6);

    public delegate void VVVRRII<T0, T1, T2, T3, T4, T5, T6>(T0 t0, T1 t1, T2 t2, ref T3 t3, ref T4 t4, in T5 t5,
        in T6 t6);

    public delegate void VVVRRRI<T0, T1, T2, T3, T4, T5, T6>(T0 t0, T1 t1, T2 t2, ref T3 t3, ref T4 t4, ref T5 t5,
        in T6 t6);

    public delegate void VVVRRRR<T0, T1, T2, T3, T4, T5, T6>(T0 t0, T1 t1, T2 t2, ref T3 t3, ref T4 t4, ref T5 t5,
        ref T6 t6);

    public delegate void VVVVIII<T0, T1, T2, T3, T4, T5, T6>(T0 t0, T1 t1, T2 t2, T3 t3, in T4 t4, in T5 t5, in T6 t6);

    public delegate void VVVVRII<T0, T1, T2, T3, T4, T5, T6>(T0 t0, T1 t1, T2 t2, T3 t3, ref T4 t4, in T5 t5, in T6 t6);

    public delegate void
        VVVVRRI<T0, T1, T2, T3, T4, T5, T6>(T0 t0, T1 t1, T2 t2, T3 t3, ref T4 t4, ref T5 t5, in T6 t6);

    public delegate void VVVVRRR<T0, T1, T2, T3, T4, T5, T6>(T0 t0, T1 t1, T2 t2, T3 t3, ref T4 t4, ref T5 t5,
        ref T6 t6);

    public delegate void VVVVVII<T0, T1, T2, T3, T4, T5, T6>(T0 t0, T1 t1, T2 t2, T3 t3, T4 t4, in T5 t5, in T6 t6);

    public delegate void VVVVVRI<T0, T1, T2, T3, T4, T5, T6>(T0 t0, T1 t1, T2 t2, T3 t3, T4 t4, ref T5 t5, in T6 t6);

    public delegate void VVVVVRR<T0, T1, T2, T3, T4, T5, T6>(T0 t0, T1 t1, T2 t2, T3 t3, T4 t4, ref T5 t5, ref T6 t6);

    public delegate void VVVVVVI<T0, T1, T2, T3, T4, T5, T6>(T0 t0, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, in T6 t6);

    public delegate void VVVVVVR<T0, T1, T2, T3, T4, T5, T6>(T0 t0, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, ref T6 t6);

    public delegate void VVVVVVV<T0, T1, T2, T3, T4, T5, T6>(T0 t0, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6);

    public delegate void RIIIIIII<T0, T1, T2, T3, T4, T5, T6, T7>(ref T0 t0, in T1 t1, in T2 t2, in T3 t3, in T4 t4,
        in T5 t5, in T6 t6, in T7 t7);

    public delegate void RRIIIIII<T0, T1, T2, T3, T4, T5, T6, T7>(ref T0 t0, ref T1 t1, in T2 t2, in T3 t3, in T4 t4,
        in T5 t5, in T6 t6, in T7 t7);

    public delegate void RRRIIIII<T0, T1, T2, T3, T4, T5, T6, T7>(ref T0 t0, ref T1 t1, ref T2 t2, in T3 t3, in T4 t4,
        in T5 t5, in T6 t6, in T7 t7);

    public delegate void RRRRIIII<T0, T1, T2, T3, T4, T5, T6, T7>(ref T0 t0, ref T1 t1, ref T2 t2, ref T3 t3, in T4 t4,
        in T5 t5, in T6 t6, in T7 t7);

    public delegate void RRRRRIII<T0, T1, T2, T3, T4, T5, T6, T7>(ref T0 t0, ref T1 t1, ref T2 t2, ref T3 t3, ref T4 t4,
        in T5 t5, in T6 t6, in T7 t7);

    public delegate void RRRRRRII<T0, T1, T2, T3, T4, T5, T6, T7>(ref T0 t0, ref T1 t1, ref T2 t2, ref T3 t3, ref T4 t4,
        ref T5 t5, in T6 t6, in T7 t7);

    public delegate void RRRRRRRI<T0, T1, T2, T3, T4, T5, T6, T7>(ref T0 t0, ref T1 t1, ref T2 t2, ref T3 t3, ref T4 t4,
        ref T5 t5, ref T6 t6, in T7 t7);

    public delegate void RRRRRRRR<T0, T1, T2, T3, T4, T5, T6, T7>(ref T0 t0, ref T1 t1, ref T2 t2, ref T3 t3, ref T4 t4,
        ref T5 t5, ref T6 t6, ref T7 t7);

    public delegate void IIIIIIII<T0, T1, T2, T3, T4, T5, T6, T7>(in T0 t0, in T1 t1, in T2 t2, in T3 t3, in T4 t4,
        in T5 t5, in T6 t6, in T7 t7);

    public delegate void VIIIIIII<T0, T1, T2, T3, T4, T5, T6, T7>(T0 t0, in T1 t1, in T2 t2, in T3 t3, in T4 t4,
        in T5 t5, in T6 t6, in T7 t7);

    public delegate void VRIIIIII<T0, T1, T2, T3, T4, T5, T6, T7>(T0 t0, ref T1 t1, in T2 t2, in T3 t3, in T4 t4,
        in T5 t5, in T6 t6, in T7 t7);

    public delegate void VRRIIIII<T0, T1, T2, T3, T4, T5, T6, T7>(T0 t0, ref T1 t1, ref T2 t2, in T3 t3, in T4 t4,
        in T5 t5, in T6 t6, in T7 t7);

    public delegate void VRRRIIII<T0, T1, T2, T3, T4, T5, T6, T7>(T0 t0, ref T1 t1, ref T2 t2, ref T3 t3, in T4 t4,
        in T5 t5, in T6 t6, in T7 t7);

    public delegate void VRRRRIII<T0, T1, T2, T3, T4, T5, T6, T7>(T0 t0, ref T1 t1, ref T2 t2, ref T3 t3, ref T4 t4,
        in T5 t5, in T6 t6, in T7 t7);

    public delegate void VRRRRRII<T0, T1, T2, T3, T4, T5, T6, T7>(T0 t0, ref T1 t1, ref T2 t2, ref T3 t3, ref T4 t4,
        ref T5 t5, in T6 t6, in T7 t7);

    public delegate void VRRRRRRI<T0, T1, T2, T3, T4, T5, T6, T7>(T0 t0, ref T1 t1, ref T2 t2, ref T3 t3, ref T4 t4,
        ref T5 t5, ref T6 t6, in T7 t7);

    public delegate void VRRRRRRR<T0, T1, T2, T3, T4, T5, T6, T7>(T0 t0, ref T1 t1, ref T2 t2, ref T3 t3, ref T4 t4,
        ref T5 t5, ref T6 t6, ref T7 t7);

    public delegate void VVIIIIII<T0, T1, T2, T3, T4, T5, T6, T7>(T0 t0, T1 t1, in T2 t2, in T3 t3, in T4 t4, in T5 t5,
        in T6 t6, in T7 t7);

    public delegate void VVRIIIII<T0, T1, T2, T3, T4, T5, T6, T7>(T0 t0, T1 t1, ref T2 t2, in T3 t3, in T4 t4, in T5 t5,
        in T6 t6, in T7 t7);

    public delegate void VVRRIIII<T0, T1, T2, T3, T4, T5, T6, T7>(T0 t0, T1 t1, ref T2 t2, ref T3 t3, in T4 t4,
        in T5 t5, in T6 t6, in T7 t7);

    public delegate void VVRRRIII<T0, T1, T2, T3, T4, T5, T6, T7>(T0 t0, T1 t1, ref T2 t2, ref T3 t3, ref T4 t4,
        in T5 t5, in T6 t6, in T7 t7);

    public delegate void VVRRRRII<T0, T1, T2, T3, T4, T5, T6, T7>(T0 t0, T1 t1, ref T2 t2, ref T3 t3, ref T4 t4,
        ref T5 t5, in T6 t6, in T7 t7);

    public delegate void VVRRRRRI<T0, T1, T2, T3, T4, T5, T6, T7>(T0 t0, T1 t1, ref T2 t2, ref T3 t3, ref T4 t4,
        ref T5 t5, ref T6 t6, in T7 t7);

    public delegate void VVRRRRRR<T0, T1, T2, T3, T4, T5, T6, T7>(T0 t0, T1 t1, ref T2 t2, ref T3 t3, ref T4 t4,
        ref T5 t5, ref T6 t6, ref T7 t7);

    public delegate void VVVIIIII<T0, T1, T2, T3, T4, T5, T6, T7>(T0 t0, T1 t1, T2 t2, in T3 t3, in T4 t4, in T5 t5,
        in T6 t6, in T7 t7);

    public delegate void VVVRIIII<T0, T1, T2, T3, T4, T5, T6, T7>(T0 t0, T1 t1, T2 t2, ref T3 t3, in T4 t4, in T5 t5,
        in T6 t6, in T7 t7);

    public delegate void VVVRRIII<T0, T1, T2, T3, T4, T5, T6, T7>(T0 t0, T1 t1, T2 t2, ref T3 t3, ref T4 t4, in T5 t5,
        in T6 t6, in T7 t7);

    public delegate void VVVRRRII<T0, T1, T2, T3, T4, T5, T6, T7>(T0 t0, T1 t1, T2 t2, ref T3 t3, ref T4 t4, ref T5 t5,
        in T6 t6, in T7 t7);

    public delegate void VVVRRRRI<T0, T1, T2, T3, T4, T5, T6, T7>(T0 t0, T1 t1, T2 t2, ref T3 t3, ref T4 t4, ref T5 t5,
        ref T6 t6, in T7 t7);

    public delegate void VVVRRRRR<T0, T1, T2, T3, T4, T5, T6, T7>(T0 t0, T1 t1, T2 t2, ref T3 t3, ref T4 t4, ref T5 t5,
        ref T6 t6, ref T7 t7);

    public delegate void VVVVIIII<T0, T1, T2, T3, T4, T5, T6, T7>(T0 t0, T1 t1, T2 t2, T3 t3, in T4 t4, in T5 t5,
        in T6 t6, in T7 t7);

    public delegate void VVVVRIII<T0, T1, T2, T3, T4, T5, T6, T7>(T0 t0, T1 t1, T2 t2, T3 t3, ref T4 t4, in T5 t5,
        in T6 t6, in T7 t7);

    public delegate void VVVVRRII<T0, T1, T2, T3, T4, T5, T6, T7>(T0 t0, T1 t1, T2 t2, T3 t3, ref T4 t4, ref T5 t5,
        in T6 t6, in T7 t7);

    public delegate void VVVVRRRI<T0, T1, T2, T3, T4, T5, T6, T7>(T0 t0, T1 t1, T2 t2, T3 t3, ref T4 t4, ref T5 t5,
        ref T6 t6, in T7 t7);

    public delegate void VVVVRRRR<T0, T1, T2, T3, T4, T5, T6, T7>(T0 t0, T1 t1, T2 t2, T3 t3, ref T4 t4, ref T5 t5,
        ref T6 t6, ref T7 t7);

    public delegate void VVVVVIII<T0, T1, T2, T3, T4, T5, T6, T7>(T0 t0, T1 t1, T2 t2, T3 t3, T4 t4, in T5 t5, in T6 t6,
        in T7 t7);

    public delegate void VVVVVRII<T0, T1, T2, T3, T4, T5, T6, T7>(T0 t0, T1 t1, T2 t2, T3 t3, T4 t4, ref T5 t5,
        in T6 t6, in T7 t7);

    public delegate void VVVVVRRI<T0, T1, T2, T3, T4, T5, T6, T7>(T0 t0, T1 t1, T2 t2, T3 t3, T4 t4, ref T5 t5,
        ref T6 t6, in T7 t7);

    public delegate void VVVVVRRR<T0, T1, T2, T3, T4, T5, T6, T7>(T0 t0, T1 t1, T2 t2, T3 t3, T4 t4, ref T5 t5,
        ref T6 t6, ref T7 t7);

    public delegate void VVVVVVII<T0, T1, T2, T3, T4, T5, T6, T7>(T0 t0, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, in T6 t6,
        in T7 t7);

    public delegate void VVVVVVRI<T0, T1, T2, T3, T4, T5, T6, T7>(T0 t0, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, ref T6 t6,
        in T7 t7);

    public delegate void VVVVVVRR<T0, T1, T2, T3, T4, T5, T6, T7>(T0 t0, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, ref T6 t6,
        ref T7 t7);

    public delegate void VVVVVVVI<T0, T1, T2, T3, T4, T5, T6, T7>(T0 t0, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6,
        in T7 t7);

    public delegate void VVVVVVVR<T0, T1, T2, T3, T4, T5, T6, T7>(T0 t0, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6,
        ref T7 t7);

    public delegate void VVVVVVVV<T0, T1, T2, T3, T4, T5, T6, T7>(T0 t0, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6,
        T7 t7);
}

public static class LambdaForEachDescriptionConstructionMethods
{
    public static TDescription ForEach<TDescription, T0>(this TDescription description, R<T0> codeToRun)
        where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0>(this TDescription description, I<T0> codeToRun)
        where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0>(this TDescription description, V<T0> codeToRun)
        where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0, T1>(this TDescription description, RI<T0, T1> codeToRun)
        where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0, T1>(this TDescription description, RR<T0, T1> codeToRun)
        where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0, T1>(this TDescription description, II<T0, T1> codeToRun)
        where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0, T1>(this TDescription description, VI<T0, T1> codeToRun)
        where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0, T1>(this TDescription description, VR<T0, T1> codeToRun)
        where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0, T1>(this TDescription description, VV<T0, T1> codeToRun)
        where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0, T1, T2>(this TDescription description,
        RII<T0, T1, T2> codeToRun) where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0, T1, T2>(this TDescription description,
        RRI<T0, T1, T2> codeToRun) where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0, T1, T2>(this TDescription description,
        RRR<T0, T1, T2> codeToRun) where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0, T1, T2>(this TDescription description,
        III<T0, T1, T2> codeToRun) where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0, T1, T2>(this TDescription description,
        VII<T0, T1, T2> codeToRun) where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0, T1, T2>(this TDescription description,
        VRI<T0, T1, T2> codeToRun) where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0, T1, T2>(this TDescription description,
        VRR<T0, T1, T2> codeToRun) where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0, T1, T2>(this TDescription description,
        VVI<T0, T1, T2> codeToRun) where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0, T1, T2>(this TDescription description,
        VVR<T0, T1, T2> codeToRun) where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0, T1, T2>(this TDescription description,
        VVV<T0, T1, T2> codeToRun) where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0, T1, T2, T3>(this TDescription description,
        RIII<T0, T1, T2, T3> codeToRun) where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0, T1, T2, T3>(this TDescription description,
        RRII<T0, T1, T2, T3> codeToRun) where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0, T1, T2, T3>(this TDescription description,
        RRRI<T0, T1, T2, T3> codeToRun) where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0, T1, T2, T3>(this TDescription description,
        RRRR<T0, T1, T2, T3> codeToRun) where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0, T1, T2, T3>(this TDescription description,
        IIII<T0, T1, T2, T3> codeToRun) where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0, T1, T2, T3>(this TDescription description,
        VIII<T0, T1, T2, T3> codeToRun) where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0, T1, T2, T3>(this TDescription description,
        VRII<T0, T1, T2, T3> codeToRun) where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0, T1, T2, T3>(this TDescription description,
        VRRI<T0, T1, T2, T3> codeToRun) where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0, T1, T2, T3>(this TDescription description,
        VRRR<T0, T1, T2, T3> codeToRun) where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0, T1, T2, T3>(this TDescription description,
        VVII<T0, T1, T2, T3> codeToRun) where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0, T1, T2, T3>(this TDescription description,
        VVRI<T0, T1, T2, T3> codeToRun) where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0, T1, T2, T3>(this TDescription description,
        VVRR<T0, T1, T2, T3> codeToRun) where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0, T1, T2, T3>(this TDescription description,
        VVVI<T0, T1, T2, T3> codeToRun) where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0, T1, T2, T3>(this TDescription description,
        VVVR<T0, T1, T2, T3> codeToRun) where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0, T1, T2, T3>(this TDescription description,
        VVVV<T0, T1, T2, T3> codeToRun) where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0, T1, T2, T3, T4>(this TDescription description,
        RIIII<T0, T1, T2, T3, T4> codeToRun) where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0, T1, T2, T3, T4>(this TDescription description,
        RRIII<T0, T1, T2, T3, T4> codeToRun) where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0, T1, T2, T3, T4>(this TDescription description,
        RRRII<T0, T1, T2, T3, T4> codeToRun) where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0, T1, T2, T3, T4>(this TDescription description,
        RRRRI<T0, T1, T2, T3, T4> codeToRun) where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0, T1, T2, T3, T4>(this TDescription description,
        RRRRR<T0, T1, T2, T3, T4> codeToRun) where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0, T1, T2, T3, T4>(this TDescription description,
        IIIII<T0, T1, T2, T3, T4> codeToRun) where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0, T1, T2, T3, T4>(this TDescription description,
        VIIII<T0, T1, T2, T3, T4> codeToRun) where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0, T1, T2, T3, T4>(this TDescription description,
        VRIII<T0, T1, T2, T3, T4> codeToRun) where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0, T1, T2, T3, T4>(this TDescription description,
        VRRII<T0, T1, T2, T3, T4> codeToRun) where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0, T1, T2, T3, T4>(this TDescription description,
        VRRRI<T0, T1, T2, T3, T4> codeToRun) where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0, T1, T2, T3, T4>(this TDescription description,
        VRRRR<T0, T1, T2, T3, T4> codeToRun) where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0, T1, T2, T3, T4>(this TDescription description,
        VVIII<T0, T1, T2, T3, T4> codeToRun) where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0, T1, T2, T3, T4>(this TDescription description,
        VVRII<T0, T1, T2, T3, T4> codeToRun) where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0, T1, T2, T3, T4>(this TDescription description,
        VVRRI<T0, T1, T2, T3, T4> codeToRun) where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0, T1, T2, T3, T4>(this TDescription description,
        VVRRR<T0, T1, T2, T3, T4> codeToRun) where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0, T1, T2, T3, T4>(this TDescription description,
        VVVII<T0, T1, T2, T3, T4> codeToRun) where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0, T1, T2, T3, T4>(this TDescription description,
        VVVRI<T0, T1, T2, T3, T4> codeToRun) where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0, T1, T2, T3, T4>(this TDescription description,
        VVVRR<T0, T1, T2, T3, T4> codeToRun) where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0, T1, T2, T3, T4>(this TDescription description,
        VVVVI<T0, T1, T2, T3, T4> codeToRun) where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0, T1, T2, T3, T4>(this TDescription description,
        VVVVR<T0, T1, T2, T3, T4> codeToRun) where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0, T1, T2, T3, T4>(this TDescription description,
        VVVVV<T0, T1, T2, T3, T4> codeToRun) where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0, T1, T2, T3, T4, T5>(this TDescription description,
        RIIIII<T0, T1, T2, T3, T4, T5> codeToRun) where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0, T1, T2, T3, T4, T5>(this TDescription description,
        RRIIII<T0, T1, T2, T3, T4, T5> codeToRun) where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0, T1, T2, T3, T4, T5>(this TDescription description,
        RRRIII<T0, T1, T2, T3, T4, T5> codeToRun) where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0, T1, T2, T3, T4, T5>(this TDescription description,
        RRRRII<T0, T1, T2, T3, T4, T5> codeToRun) where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0, T1, T2, T3, T4, T5>(this TDescription description,
        RRRRRI<T0, T1, T2, T3, T4, T5> codeToRun) where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0, T1, T2, T3, T4, T5>(this TDescription description,
        RRRRRR<T0, T1, T2, T3, T4, T5> codeToRun) where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0, T1, T2, T3, T4, T5>(this TDescription description,
        IIIIII<T0, T1, T2, T3, T4, T5> codeToRun) where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0, T1, T2, T3, T4, T5>(this TDescription description,
        VIIIII<T0, T1, T2, T3, T4, T5> codeToRun) where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0, T1, T2, T3, T4, T5>(this TDescription description,
        VRIIII<T0, T1, T2, T3, T4, T5> codeToRun) where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0, T1, T2, T3, T4, T5>(this TDescription description,
        VRRIII<T0, T1, T2, T3, T4, T5> codeToRun) where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0, T1, T2, T3, T4, T5>(this TDescription description,
        VRRRII<T0, T1, T2, T3, T4, T5> codeToRun) where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0, T1, T2, T3, T4, T5>(this TDescription description,
        VRRRRI<T0, T1, T2, T3, T4, T5> codeToRun) where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0, T1, T2, T3, T4, T5>(this TDescription description,
        VRRRRR<T0, T1, T2, T3, T4, T5> codeToRun) where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0, T1, T2, T3, T4, T5>(this TDescription description,
        VVIIII<T0, T1, T2, T3, T4, T5> codeToRun) where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0, T1, T2, T3, T4, T5>(this TDescription description,
        VVRIII<T0, T1, T2, T3, T4, T5> codeToRun) where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0, T1, T2, T3, T4, T5>(this TDescription description,
        VVRRII<T0, T1, T2, T3, T4, T5> codeToRun) where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0, T1, T2, T3, T4, T5>(this TDescription description,
        VVRRRI<T0, T1, T2, T3, T4, T5> codeToRun) where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0, T1, T2, T3, T4, T5>(this TDescription description,
        VVRRRR<T0, T1, T2, T3, T4, T5> codeToRun) where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0, T1, T2, T3, T4, T5>(this TDescription description,
        VVVIII<T0, T1, T2, T3, T4, T5> codeToRun) where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0, T1, T2, T3, T4, T5>(this TDescription description,
        VVVRII<T0, T1, T2, T3, T4, T5> codeToRun) where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0, T1, T2, T3, T4, T5>(this TDescription description,
        VVVRRI<T0, T1, T2, T3, T4, T5> codeToRun) where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0, T1, T2, T3, T4, T5>(this TDescription description,
        VVVRRR<T0, T1, T2, T3, T4, T5> codeToRun) where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0, T1, T2, T3, T4, T5>(this TDescription description,
        VVVVII<T0, T1, T2, T3, T4, T5> codeToRun) where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0, T1, T2, T3, T4, T5>(this TDescription description,
        VVVVRI<T0, T1, T2, T3, T4, T5> codeToRun) where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0, T1, T2, T3, T4, T5>(this TDescription description,
        VVVVRR<T0, T1, T2, T3, T4, T5> codeToRun) where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0, T1, T2, T3, T4, T5>(this TDescription description,
        VVVVVI<T0, T1, T2, T3, T4, T5> codeToRun) where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0, T1, T2, T3, T4, T5>(this TDescription description,
        VVVVVR<T0, T1, T2, T3, T4, T5> codeToRun) where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0, T1, T2, T3, T4, T5>(this TDescription description,
        VVVVVV<T0, T1, T2, T3, T4, T5> codeToRun) where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0, T1, T2, T3, T4, T5, T6>(this TDescription description,
        RIIIIII<T0, T1, T2, T3, T4, T5, T6> codeToRun) where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0, T1, T2, T3, T4, T5, T6>(this TDescription description,
        RRIIIII<T0, T1, T2, T3, T4, T5, T6> codeToRun) where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0, T1, T2, T3, T4, T5, T6>(this TDescription description,
        RRRIIII<T0, T1, T2, T3, T4, T5, T6> codeToRun) where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0, T1, T2, T3, T4, T5, T6>(this TDescription description,
        RRRRIII<T0, T1, T2, T3, T4, T5, T6> codeToRun) where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0, T1, T2, T3, T4, T5, T6>(this TDescription description,
        RRRRRII<T0, T1, T2, T3, T4, T5, T6> codeToRun) where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0, T1, T2, T3, T4, T5, T6>(this TDescription description,
        RRRRRRI<T0, T1, T2, T3, T4, T5, T6> codeToRun) where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0, T1, T2, T3, T4, T5, T6>(this TDescription description,
        RRRRRRR<T0, T1, T2, T3, T4, T5, T6> codeToRun) where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0, T1, T2, T3, T4, T5, T6>(this TDescription description,
        IIIIIII<T0, T1, T2, T3, T4, T5, T6> codeToRun) where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0, T1, T2, T3, T4, T5, T6>(this TDescription description,
        VIIIIII<T0, T1, T2, T3, T4, T5, T6> codeToRun) where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0, T1, T2, T3, T4, T5, T6>(this TDescription description,
        VRIIIII<T0, T1, T2, T3, T4, T5, T6> codeToRun) where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0, T1, T2, T3, T4, T5, T6>(this TDescription description,
        VRRIIII<T0, T1, T2, T3, T4, T5, T6> codeToRun) where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0, T1, T2, T3, T4, T5, T6>(this TDescription description,
        VRRRIII<T0, T1, T2, T3, T4, T5, T6> codeToRun) where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0, T1, T2, T3, T4, T5, T6>(this TDescription description,
        VRRRRII<T0, T1, T2, T3, T4, T5, T6> codeToRun) where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0, T1, T2, T3, T4, T5, T6>(this TDescription description,
        VRRRRRI<T0, T1, T2, T3, T4, T5, T6> codeToRun) where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0, T1, T2, T3, T4, T5, T6>(this TDescription description,
        VRRRRRR<T0, T1, T2, T3, T4, T5, T6> codeToRun) where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0, T1, T2, T3, T4, T5, T6>(this TDescription description,
        VVIIIII<T0, T1, T2, T3, T4, T5, T6> codeToRun) where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0, T1, T2, T3, T4, T5, T6>(this TDescription description,
        VVRIIII<T0, T1, T2, T3, T4, T5, T6> codeToRun) where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0, T1, T2, T3, T4, T5, T6>(this TDescription description,
        VVRRIII<T0, T1, T2, T3, T4, T5, T6> codeToRun) where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0, T1, T2, T3, T4, T5, T6>(this TDescription description,
        VVRRRII<T0, T1, T2, T3, T4, T5, T6> codeToRun) where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0, T1, T2, T3, T4, T5, T6>(this TDescription description,
        VVRRRRI<T0, T1, T2, T3, T4, T5, T6> codeToRun) where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0, T1, T2, T3, T4, T5, T6>(this TDescription description,
        VVRRRRR<T0, T1, T2, T3, T4, T5, T6> codeToRun) where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0, T1, T2, T3, T4, T5, T6>(this TDescription description,
        VVVIIII<T0, T1, T2, T3, T4, T5, T6> codeToRun) where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0, T1, T2, T3, T4, T5, T6>(this TDescription description,
        VVVRIII<T0, T1, T2, T3, T4, T5, T6> codeToRun) where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0, T1, T2, T3, T4, T5, T6>(this TDescription description,
        VVVRRII<T0, T1, T2, T3, T4, T5, T6> codeToRun) where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0, T1, T2, T3, T4, T5, T6>(this TDescription description,
        VVVRRRI<T0, T1, T2, T3, T4, T5, T6> codeToRun) where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0, T1, T2, T3, T4, T5, T6>(this TDescription description,
        VVVRRRR<T0, T1, T2, T3, T4, T5, T6> codeToRun) where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0, T1, T2, T3, T4, T5, T6>(this TDescription description,
        VVVVIII<T0, T1, T2, T3, T4, T5, T6> codeToRun) where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0, T1, T2, T3, T4, T5, T6>(this TDescription description,
        VVVVRII<T0, T1, T2, T3, T4, T5, T6> codeToRun) where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0, T1, T2, T3, T4, T5, T6>(this TDescription description,
        VVVVRRI<T0, T1, T2, T3, T4, T5, T6> codeToRun) where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0, T1, T2, T3, T4, T5, T6>(this TDescription description,
        VVVVRRR<T0, T1, T2, T3, T4, T5, T6> codeToRun) where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0, T1, T2, T3, T4, T5, T6>(this TDescription description,
        VVVVVII<T0, T1, T2, T3, T4, T5, T6> codeToRun) where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0, T1, T2, T3, T4, T5, T6>(this TDescription description,
        VVVVVRI<T0, T1, T2, T3, T4, T5, T6> codeToRun) where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0, T1, T2, T3, T4, T5, T6>(this TDescription description,
        VVVVVRR<T0, T1, T2, T3, T4, T5, T6> codeToRun) where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0, T1, T2, T3, T4, T5, T6>(this TDescription description,
        VVVVVVI<T0, T1, T2, T3, T4, T5, T6> codeToRun) where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0, T1, T2, T3, T4, T5, T6>(this TDescription description,
        VVVVVVR<T0, T1, T2, T3, T4, T5, T6> codeToRun) where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0, T1, T2, T3, T4, T5, T6>(this TDescription description,
        VVVVVVV<T0, T1, T2, T3, T4, T5, T6> codeToRun) where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0, T1, T2, T3, T4, T5, T6, T7>(this TDescription description,
        RIIIIIII<T0, T1, T2, T3, T4, T5, T6, T7> codeToRun)
        where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0, T1, T2, T3, T4, T5, T6, T7>(this TDescription description,
        RRIIIIII<T0, T1, T2, T3, T4, T5, T6, T7> codeToRun)
        where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0, T1, T2, T3, T4, T5, T6, T7>(this TDescription description,
        RRRIIIII<T0, T1, T2, T3, T4, T5, T6, T7> codeToRun)
        where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0, T1, T2, T3, T4, T5, T6, T7>(this TDescription description,
        RRRRIIII<T0, T1, T2, T3, T4, T5, T6, T7> codeToRun)
        where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0, T1, T2, T3, T4, T5, T6, T7>(this TDescription description,
        RRRRRIII<T0, T1, T2, T3, T4, T5, T6, T7> codeToRun)
        where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0, T1, T2, T3, T4, T5, T6, T7>(this TDescription description,
        RRRRRRII<T0, T1, T2, T3, T4, T5, T6, T7> codeToRun)
        where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0, T1, T2, T3, T4, T5, T6, T7>(this TDescription description,
        RRRRRRRI<T0, T1, T2, T3, T4, T5, T6, T7> codeToRun)
        where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0, T1, T2, T3, T4, T5, T6, T7>(this TDescription description,
        RRRRRRRR<T0, T1, T2, T3, T4, T5, T6, T7> codeToRun)
        where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0, T1, T2, T3, T4, T5, T6, T7>(this TDescription description,
        IIIIIIII<T0, T1, T2, T3, T4, T5, T6, T7> codeToRun)
        where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0, T1, T2, T3, T4, T5, T6, T7>(this TDescription description,
        VIIIIIII<T0, T1, T2, T3, T4, T5, T6, T7> codeToRun)
        where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0, T1, T2, T3, T4, T5, T6, T7>(this TDescription description,
        VRIIIIII<T0, T1, T2, T3, T4, T5, T6, T7> codeToRun)
        where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0, T1, T2, T3, T4, T5, T6, T7>(this TDescription description,
        VRRIIIII<T0, T1, T2, T3, T4, T5, T6, T7> codeToRun)
        where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0, T1, T2, T3, T4, T5, T6, T7>(this TDescription description,
        VRRRIIII<T0, T1, T2, T3, T4, T5, T6, T7> codeToRun)
        where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0, T1, T2, T3, T4, T5, T6, T7>(this TDescription description,
        VRRRRIII<T0, T1, T2, T3, T4, T5, T6, T7> codeToRun)
        where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0, T1, T2, T3, T4, T5, T6, T7>(this TDescription description,
        VRRRRRII<T0, T1, T2, T3, T4, T5, T6, T7> codeToRun)
        where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0, T1, T2, T3, T4, T5, T6, T7>(this TDescription description,
        VRRRRRRI<T0, T1, T2, T3, T4, T5, T6, T7> codeToRun)
        where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0, T1, T2, T3, T4, T5, T6, T7>(this TDescription description,
        VRRRRRRR<T0, T1, T2, T3, T4, T5, T6, T7> codeToRun)
        where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0, T1, T2, T3, T4, T5, T6, T7>(this TDescription description,
        VVIIIIII<T0, T1, T2, T3, T4, T5, T6, T7> codeToRun)
        where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0, T1, T2, T3, T4, T5, T6, T7>(this TDescription description,
        VVRIIIII<T0, T1, T2, T3, T4, T5, T6, T7> codeToRun)
        where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0, T1, T2, T3, T4, T5, T6, T7>(this TDescription description,
        VVRRIIII<T0, T1, T2, T3, T4, T5, T6, T7> codeToRun)
        where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0, T1, T2, T3, T4, T5, T6, T7>(this TDescription description,
        VVRRRIII<T0, T1, T2, T3, T4, T5, T6, T7> codeToRun)
        where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0, T1, T2, T3, T4, T5, T6, T7>(this TDescription description,
        VVRRRRII<T0, T1, T2, T3, T4, T5, T6, T7> codeToRun)
        where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0, T1, T2, T3, T4, T5, T6, T7>(this TDescription description,
        VVRRRRRI<T0, T1, T2, T3, T4, T5, T6, T7> codeToRun)
        where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0, T1, T2, T3, T4, T5, T6, T7>(this TDescription description,
        VVRRRRRR<T0, T1, T2, T3, T4, T5, T6, T7> codeToRun)
        where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0, T1, T2, T3, T4, T5, T6, T7>(this TDescription description,
        VVVIIIII<T0, T1, T2, T3, T4, T5, T6, T7> codeToRun)
        where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0, T1, T2, T3, T4, T5, T6, T7>(this TDescription description,
        VVVRIIII<T0, T1, T2, T3, T4, T5, T6, T7> codeToRun)
        where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0, T1, T2, T3, T4, T5, T6, T7>(this TDescription description,
        VVVRRIII<T0, T1, T2, T3, T4, T5, T6, T7> codeToRun)
        where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0, T1, T2, T3, T4, T5, T6, T7>(this TDescription description,
        VVVRRRII<T0, T1, T2, T3, T4, T5, T6, T7> codeToRun)
        where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0, T1, T2, T3, T4, T5, T6, T7>(this TDescription description,
        VVVRRRRI<T0, T1, T2, T3, T4, T5, T6, T7> codeToRun)
        where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0, T1, T2, T3, T4, T5, T6, T7>(this TDescription description,
        VVVRRRRR<T0, T1, T2, T3, T4, T5, T6, T7> codeToRun)
        where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0, T1, T2, T3, T4, T5, T6, T7>(this TDescription description,
        VVVVIIII<T0, T1, T2, T3, T4, T5, T6, T7> codeToRun)
        where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0, T1, T2, T3, T4, T5, T6, T7>(this TDescription description,
        VVVVRIII<T0, T1, T2, T3, T4, T5, T6, T7> codeToRun)
        where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0, T1, T2, T3, T4, T5, T6, T7>(this TDescription description,
        VVVVRRII<T0, T1, T2, T3, T4, T5, T6, T7> codeToRun)
        where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0, T1, T2, T3, T4, T5, T6, T7>(this TDescription description,
        VVVVRRRI<T0, T1, T2, T3, T4, T5, T6, T7> codeToRun)
        where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0, T1, T2, T3, T4, T5, T6, T7>(this TDescription description,
        VVVVRRRR<T0, T1, T2, T3, T4, T5, T6, T7> codeToRun)
        where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0, T1, T2, T3, T4, T5, T6, T7>(this TDescription description,
        VVVVVIII<T0, T1, T2, T3, T4, T5, T6, T7> codeToRun)
        where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0, T1, T2, T3, T4, T5, T6, T7>(this TDescription description,
        VVVVVRII<T0, T1, T2, T3, T4, T5, T6, T7> codeToRun)
        where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0, T1, T2, T3, T4, T5, T6, T7>(this TDescription description,
        VVVVVRRI<T0, T1, T2, T3, T4, T5, T6, T7> codeToRun)
        where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0, T1, T2, T3, T4, T5, T6, T7>(this TDescription description,
        VVVVVRRR<T0, T1, T2, T3, T4, T5, T6, T7> codeToRun)
        where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0, T1, T2, T3, T4, T5, T6, T7>(this TDescription description,
        VVVVVVII<T0, T1, T2, T3, T4, T5, T6, T7> codeToRun)
        where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0, T1, T2, T3, T4, T5, T6, T7>(this TDescription description,
        VVVVVVRI<T0, T1, T2, T3, T4, T5, T6, T7> codeToRun)
        where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0, T1, T2, T3, T4, T5, T6, T7>(this TDescription description,
        VVVVVVRR<T0, T1, T2, T3, T4, T5, T6, T7> codeToRun)
        where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0, T1, T2, T3, T4, T5, T6, T7>(this TDescription description,
        VVVVVVVI<T0, T1, T2, T3, T4, T5, T6, T7> codeToRun)
        where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0, T1, T2, T3, T4, T5, T6, T7>(this TDescription description,
        VVVVVVVR<T0, T1, T2, T3, T4, T5, T6, T7> codeToRun)
        where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ForEach<TDescription, T0, T1, T2, T3, T4, T5, T6, T7>(this TDescription description,
        VVVVVVVV<T0, T1, T2, T3, T4, T5, T6, T7> codeToRun)
        where TDescription : struct, ISupportForEachWithUniversalDelegate
    {
        return ThrowCodeGenException<TDescription>();
    }

    public static TDescription ThrowCodeGenException<TDescription>()
    {
        throw new NotImplementedException($"Code is not generated for {typeof(TDescription)}");
    }
}