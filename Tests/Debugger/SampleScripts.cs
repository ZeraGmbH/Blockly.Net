namespace BlocklyNetTests.Debugger;

public static class SampleScripts
{
    public const string DebugScript1 = @"<xml xmlns=""https://developers.google.com/blockly/xml"">
        <variables>
            <variable id=""R5zu[aNP6Iw*48om6KC3"">result</variable>
            <variable id=""G%xy.r-U$Z?lf/@R_`+9"">i</variable>
        </variables>
        <block type=""variables_set"" id=""~-@g:c_wcw/=l7I4Z$X9"" x=""125"" y=""125"">
            <field name=""VAR"" id=""R5zu[aNP6Iw*48om6KC3"">result</field>
            <value name=""VALUE"">
                <block type=""math_number"" id=""lIvo!.Jrw(mvKuaEVo2="">
                    <field name=""NUM"">0</field>
                </block>
            </value>
            <next>
                <block type=""controls_for"" id=""Kb__-_**cI=OxUIZg9yW"">
                    <field name=""VAR"" id=""G%xy.r-U$Z?lf/@R_`+9"">i</field>
                    <value name=""FROM"">
                        <shadow type=""math_number"" id=""2k6SH;1E.|ih-%#R%bn7"">
                            <field name=""NUM"">1</field>
                        </shadow>
                    </value>
                    <value name=""TO"">
                        <shadow type=""math_number"" id=""X]T68}V`x=YKWwbHi{TS"">
                            <field name=""NUM"">1000</field>
                        </shadow>
                    </value>
                    <value name=""BY"">
                        <shadow type=""math_number"" id=""2[+5)H.j[KS@E]5,P($s"">
                            <field name=""NUM"">1</field>
                        </shadow>
                    </value>
                    <statement name=""DO"">
                        <block type=""variables_set"" id=""E4zG%:vurlQU}y:TTkl1"">
                            <field name=""VAR"" id=""R5zu[aNP6Iw*48om6KC3"">result</field>
                            <value name=""VALUE"">
                                <block type=""math_arithmetic"" id=""X/i3:*Zxs6B(Y!IGPoEi"">
                                    <field name=""OP"">ADD</field>
                                    <value name=""A"">
                                        <shadow type=""math_number"" id=""V5zO86Z9HTs3F^oWt}m5"">
                                            <field name=""NUM"">1</field>
                                        </shadow>
                                        <block type=""variables_get"" id=""MJftZL9Aaqjse7k]@o{?"">
                                            <field name=""VAR"" id=""R5zu[aNP6Iw*48om6KC3"">result</field>
                                        </block>
                                    </value>
                                    <value name=""B"">
                                        <shadow type=""math_number"" id=""r=TaG7oAsftyM=ex%Bx="">
                                            <field name=""NUM"">1</field>
                                        </shadow>
                                        <block type=""variables_get"" id=""IdwUT-cw6*BO,#8@`?2}"">
                                            <field name=""VAR"" id=""G%xy.r-U$Z?lf/@R_`+9"">i</field>
                                        </block>
                                    </value>
                                </block>
                            </value>
                        </block>
                        </statement>
                </block>
            </next>
        </block>
    </xml>";

    public const string DebugScript2 = @"<xml xmlns=""https://developers.google.com/blockly/xml"">
        <variables>
            <variable id=""g8@EO|Q%.jJ-#fcj/cV-"">result</variable>
        </variables>
        <block type=""variables_set"" id=""75}(pHFe:wTIN?;vgpgZ"" x=""125"" y=""125"">
            <field name=""VAR"" id=""g8@EO|Q%.jJ-#fcj/cV-"">result</field>
            <value name=""VALUE"">
                <block type=""math_number"" id=""lI7Vl*}FOzY-%/Wp0Cbg"">
                    <field name=""NUM"">1</field>
                </block>
            </value>
            <next>
                <block type=""try_catch_finally"" id=""z0E/Pd-PhE9C#g7~TNWM"">
                    <statement name=""TRY"">
                        <block type=""throw_exception"" id=""Nt@G*FyEEVj86b~q}]b1"">
                            <field name=""MESSAGE"">Message</field>
                            <value name=""MESSAGE"">
                                <shadow type=""text"" id=""gm#?~gYGCD^o,=./PB,/"">
                                    <field name=""TEXT"">Force to 2</field>
                                </shadow>
                            </value>
                        </block>
                    </statement>
                    <statement name=""CATCH"">
                        <block type=""variables_set"" id=""oJmYgMp$8Zfj`ql2ww}1"">
                            <field name=""VAR"" id=""g8@EO|Q%.jJ-#fcj/cV-"">result</field>
                            <value name=""VALUE"">
                                <block type=""math_number"" id=""fhaFDwxQh_ktNO/FFWBO"">
                                    <field name=""NUM"">2</field>
                                </block>
                            </value>
                        </block>
                    </statement>
                </block>
            </next>
        </block>
    </xml>";

    public const string DebugScript3 = @"<xml xmlns=""https://developers.google.com/blockly/xml"">
        <variables>
            <variable id=""|7ycSckaL*`Lq(G)uZW:"">n</variable>
            <variable id=""g8@EO|Q%.jJ-#fcj/cV-"">result</variable>
            <variable id=""@,@tC$/G-,;?)~%1ojYD"">gen</variable>
            <variable id=""_j)E[G,WvCU+RTh0C$QM"">temp</variable>
            <variable id=""Z`Yuiu%`oz*2FC7;0Y`E"">i</variable>
        </variables>
        <block type=""procedures_defreturn"" id=""]W2/i`bXXn0WG?H@S$9B"" x=""675"" y=""125"">
            <field name=""NAME"">generate</field>
            <comment pinned=""false"" h=""80"" w=""160"">Describe this function...</comment>
            <statement name=""STACK"">
            <block type=""variables_set"" id=""G5]l)A/yzjQbrq)Cr57t"">
                <field name=""VAR"" id=""@,@tC$/G-,;?)~%1ojYD"">gen</field>
                <value name=""VALUE"">
                <block type=""math_constant"" id=""]/a.97[We!,m,/MI$ZZ7"">
                    <field name=""CONSTANT"">PI</field>
                </block>
                </value>
                <next>
                <block type=""variables_set"" id=""wHRigU}C**wp)haZMChd"">
                    <field name=""VAR"" id=""@,@tC$/G-,;?)~%1ojYD"">gen</field>
                    <value name=""VALUE"">
                    <block type=""math_single"" id=""*$q?Ygr4,?mjN=]usCUG"">
                        <field name=""OP"">ROOT</field>
                        <value name=""NUM"">
                        <shadow type=""math_number"" id=""-2Ax)YE@;eJ(Q)$V]]f~"">
                            <field name=""NUM"">9</field>
                        </shadow>
                        <block type=""variables_get"" id=""t6?^G9y:U|]H;AzV``S?"">
                            <field name=""VAR"" id=""@,@tC$/G-,;?)~%1ojYD"">gen</field>
                        </block>
                        </value>
                    </block>
                    </value>
                </block>
                </next>
            </block>
            </statement>
            <value name=""RETURN"">
            <block type=""math_arithmetic"" id=""3:~tZ:??Z|_@!XeP@QbK"">
                <field name=""OP"">ADD</field>
                <value name=""A"">
                <shadow type=""math_number"" id=""VXI_h;BDSzCoL?muw*zh"">
                    <field name=""NUM"">1</field>
                </shadow>
                <block type=""variables_get"" id=""j+8zsVf)-9J%{epog3yJ"">
                    <field name=""VAR"" id=""@,@tC$/G-,;?)~%1ojYD"">gen</field>
                </block>
                </value>
                <value name=""B"">
                <shadow type=""math_number"" id=""4fSeixJ9S(#@8+^m`Jw?"">
                    <field name=""NUM"">1</field>
                </shadow>
                </value>
            </block>
            </value>
        </block>
        <block type=""variables_set"" id=""zwqv-n16yb;rL[HE:.:`"" x=""125"" y=""275"">
            <field name=""VAR"" id=""g8@EO|Q%.jJ-#fcj/cV-"">result</field>
            <value name=""VALUE"">
            <block type=""procedures_callreturn"" id=""rwLhp%#bQJ$/{tc7RSH="">
                <mutation name=""sumTo"">
                <arg name=""n""></arg>
                </mutation>
                <value name=""ARG0"">
                <block type=""math_number"" id=""Yx(j1I#|[HL@xSnu}6{~"">
                    <field name=""NUM"">13</field>
                </block>
                </value>
            </block>
            </value>
            <next>
            <block type=""variables_set"" id=""sNfTZ[N@*YOpXCOZUZEc"">
                <field name=""VAR"" id=""g8@EO|Q%.jJ-#fcj/cV-"">result</field>
                <value name=""VALUE"">
                <block type=""math_round"" id=""!%enQ4[j?:ms*?|mam^]"">
                    <field name=""OP"">ROUND</field>
                    <value name=""NUM"">
                    <shadow type=""math_number"" id=""z?|`,:?eK]~vZVeF$vEb"">
                        <field name=""NUM"">3.1</field>
                    </shadow>
                    <block type=""variables_get"" id=""fE/1ygBf){H6paq^aI;h"">
                        <field name=""VAR"" id=""g8@EO|Q%.jJ-#fcj/cV-"">result</field>
                    </block>
                    </value>
                </block>
                </value>
            </block>
            </next>
        </block>
        <block type=""procedures_defreturn"" id=""?qJ4-~q?ic@AfeMsyLI}"" x=""675"" y=""325"">
            <mutation>
            <arg name=""n"" varid=""|7ycSckaL*`Lq(G)uZW:""></arg>
            </mutation>
            <field name=""NAME"">sumTo</field>
            <comment pinned=""false"" h=""80"" w=""160"">Describe this function...</comment>
            <statement name=""STACK"">
            <block type=""variables_set"" id="")~2ixzdJ@V#!!McTY:s6"">
                <field name=""VAR"" id=""_j)E[G,WvCU+RTh0C$QM"">temp</field>
                <value name=""VALUE"">
                <block type=""math_number"" id=""rYSd]FXPGHiCcc@bBj4:"">
                    <field name=""NUM"">0</field>
                </block>
                </value>
                <next>
                <block type=""controls_for"" id=""@4q7|T[d7eF^]N5vntPA"">
                    <field name=""VAR"" id=""Z`Yuiu%`oz*2FC7;0Y`E"">i</field>
                    <value name=""FROM"">
                    <shadow type=""math_number"" id=""wj~twQQ*@PHkGPRF^A;@"">
                        <field name=""NUM"">1</field>
                    </shadow>
                    </value>
                    <value name=""TO"">
                    <shadow type=""math_number"" id=""XL^1+[oAsIna7N!|#)9Y"">
                        <field name=""NUM"">10</field>
                    </shadow>
                    <block type=""variables_get"" id=""ykHR`VIPUYQB4Bc|A8a)"">
                        <field name=""VAR"" id=""|7ycSckaL*`Lq(G)uZW:"">n</field>
                    </block>
                    </value>
                    <value name=""BY"">
                    <shadow type=""math_number"" id=""dm](%,s7@~?{.O)/v+6a"">
                        <field name=""NUM"">1</field>
                    </shadow>
                    </value>
                    <statement name=""DO"">
                    <block type=""variables_set"" id=""{U1[ES?Fxa;3EP2%Sk-F"">
                        <field name=""VAR"" id=""_j)E[G,WvCU+RTh0C$QM"">temp</field>
                        <value name=""VALUE"">
                        <block type=""math_arithmetic"" id=""!q`)@RP51[vPTACSc!3|"">
                            <field name=""OP"">ADD</field>
                            <value name=""A"">
                            <shadow type=""math_number"" id=""=Ua`o^Sla[GIG@%aj5!e"">
                                <field name=""NUM"">1</field>
                            </shadow>
                            <block type=""variables_get"" id=""AaLZhr4NuwO*=wxjab8B"">
                                <field name=""VAR"" id=""_j)E[G,WvCU+RTh0C$QM"">temp</field>
                            </block>
                            </value>
                            <value name=""B"">
                            <shadow type=""math_number"" id=""AIsQ_L,N|F}%HM(`^n2+"">
                                <field name=""NUM"">1</field>
                            </shadow>
                            <block type=""procedures_callreturn"" id=""$%v6=0*|vNRKgbn(|(n;"">
                                <mutation name=""generate""></mutation>
                            </block>
                            </value>
                        </block>
                        </value>
                        <next>
                        <block type=""variables_set"" id="";g!0eq5)ITw{V(s*tSW/"">
                            <field name=""VAR"" id=""_j)E[G,WvCU+RTh0C$QM"">temp</field>
                            <value name=""VALUE"">
                            <block type=""math_arithmetic"" id=""~cFu`Cx(b4P}(r[Jq7;b"">
                                <field name=""OP"">MULTIPLY</field>
                                <value name=""A"">
                                <shadow type=""math_number"" id=""=Ua`o^Sla[GIG@%aj5!e"">
                                    <field name=""NUM"">1</field>
                                </shadow>
                                <block type=""variables_get"" id=""NNVo~^!=G9n3bRs2PBYE"">
                                    <field name=""VAR"" id=""_j)E[G,WvCU+RTh0C$QM"">temp</field>
                                </block>
                                </value>
                                <value name=""B"">
                                <shadow type=""math_number"" id=""AIsQ_L,N|F}%HM(`^n2+"">
                                    <field name=""NUM"">1</field>
                                </shadow>
                                <block type=""math_number"" id=""_,qb1QF-%;y0zTc{dn9)"">
                                    <field name=""NUM"">1.5</field>
                                </block>
                                </value>
                            </block>
                            </value>
                        </block>
                        </next>
                    </block>
                    </statement>
                </block>
                </next>
            </block>
            </statement>
            <value name=""RETURN"">
            <block type=""variables_get"" id=""4J1$?q$yGBPD{_(.Drjn"">
                <field name=""VAR"" id=""_j)E[G,WvCU+RTh0C$QM"">temp</field>
            </block>
            </value>
        </block>
    </xml>";

    public const string DebugScript4Inner = @"<xml xmlns=""https://developers.google.com/blockly/xml"">
        <variables>
            <variable id=""g8@EO|Q%.jJ-#fcj/cV-"">result</variable>
            <variable id=""Z`Yuiu%`oz*2FC7;0Y`E"">i</variable>
        </variables>
        <block type=""variables_set"" id=""zwqv-n16yb;rL[HE:.:`"" x=""125"" y=""275"">
            <field name=""VAR"" id=""g8@EO|Q%.jJ-#fcj/cV-"">result</field>
            <value name=""VALUE"">
            <block type=""math_number"" id=""Yx(j1I#|[HL@xSnu}6{~"">
                <field name=""NUM"">0</field>
            </block>
            </value>
            <next>
            <block type=""controls_for"" id=""q]XUupsj?9w|}qKS2=cC"">
                <field name=""VAR"" id=""Z`Yuiu%`oz*2FC7;0Y`E"">i</field>
                <value name=""FROM"">
                <shadow type=""math_number"" id=""Cm|I/mWNC(vvy4CWlRbQ"">
                    <field name=""NUM"">1</field>
                </shadow>
                </value>
                <value name=""TO"">
                <shadow type=""math_number"" id=""Gx,I7II[3i^-x{aw;f.R"">
                    <field name=""NUM"">10</field>
                </shadow>
                </value>
                <value name=""BY"">
                <shadow type=""math_number"" id=""I(ciQFMhWo}KxS*S[Ify"">
                    <field name=""NUM"">1</field>
                </shadow>
                </value>
                <statement name=""DO"">
                <block type=""variables_set"" id=""cO%UQrHgNG6FZ2TBA34B"">
                    <field name=""VAR"" id=""g8@EO|Q%.jJ-#fcj/cV-"">result</field>
                    <value name=""VALUE"">
                    <block type=""math_arithmetic"" id=""y)r_+0y{i2P5,[dcI[q+"">
                        <field name=""OP"">ADD</field>
                        <value name=""A"">
                        <shadow type=""math_number"" id=""g0{k^ZA:p?e*V;BVJDhs"">
                            <field name=""NUM"">1</field>
                        </shadow>
                        <block type=""variables_get"" id=""xkD99LE]XI:;%f=u}1o@"">
                            <field name=""VAR"" id=""g8@EO|Q%.jJ-#fcj/cV-"">result</field>
                        </block>
                        </value>
                        <value name=""B"">
                        <shadow type=""math_number"" id=""arSUQk+kGOzhLiv4}rBO"">
                            <field name=""NUM"">1</field>
                        </shadow>
                        <block type=""variables_get"" id=""Z*dM$.4KJX*@?2L#Ll~^"">
                            <field name=""VAR"" id=""Z`Yuiu%`oz*2FC7;0Y`E"">i</field>
                        </block>
                        </value>
                    </block>
                    </value>
                </block>
                </statement>
            </block>
            </next>
        </block>
    </xml>";

    public static string DebugScript4Outer(string innerId) => @"<xml xmlns=""https://developers.google.com/blockly/xml"">
        <variables>
            <variable id=""g8@EO|Q%.jJ-#fcj/cV-"">result</variable>
        </variables>
        <block type=""variables_set"" id=""zwqv-n16yb;rL[HE:.:`"" x=""125"" y=""275"">
            <field name=""VAR"" id=""g8@EO|Q%.jJ-#fcj/cV-"">result</field>
            <value name=""VALUE"">
            <block type=""run_script_by_name"" id=""~^a*CB=),uB)qxsq!9iV"">
                <value name=""NAME"">
                <shadow type=""text"" id=""%oD~%SU-LZyer%n[mi=#"">
                    <field name=""TEXT""></field>
                </shadow>
                <block type=""text"" id=""E;;0,WhDABCh|lNX@]$j"">
                    <field name=""TEXT"">Jochen - Debug Demo 4 (Inner)</field>
                </block>
                </value>
                <value name=""ID"">
                <block type=""text"" id="":zl6UX-1KwCn)W%pwf14"">
                    <field name=""TEXT"">$$InnerId$$</field>
                </block>
                </value>
                <value name=""BUILDONLY"">
                <shadow type=""logic_boolean"" id=""[2:$%^!R.[helM-LWT!S"">
                    <field name=""BOOL"">FALSE</field>
                </shadow>
                </value>
            </block>
            </value>
            <next>
            <block type=""variables_set"" id=""+%/-8RI^@6ZZo+~J#rV*"">
                <field name=""VAR"" id=""g8@EO|Q%.jJ-#fcj/cV-"">result</field>
                <value name=""VALUE"">
                <block type=""math_arithmetic"" id=""Y[h*g-iK^9{djQrK(B}V"">
                    <field name=""OP"">MINUS</field>
                    <value name=""A"">
                    <shadow type=""math_number"" id=""6V_SdNuvGZou7U(zK`(v"">
                        <field name=""NUM"">0</field>
                    </shadow>
                    </value>
                    <value name=""B"">
                    <shadow type=""math_number"" id=""9lVQw3`z$aa!2y.Emt14"">
                        <field name=""NUM"">1</field>
                    </shadow>
                    <block type=""variables_get"" id=""Pl9^[(*$BJ,0.p[n5;.E"">
                        <field name=""VAR"" id=""g8@EO|Q%.jJ-#fcj/cV-"">result</field>
                    </block>
                    </value>
                </block>
                </value>
            </block>
            </next>
        </block>
    </xml>".Replace("$$InnerId$$", innerId);

    public const string DebugScript5Inner = @"<xml xmlns=""https://developers.google.com/blockly/xml"">
        <variables>
            <variable id=""ZDIXGT%Z(UJRi^~RieuB"">list</variable>
            <variable id=""QkoPNb%d.!LDp_agPt.M"">i</variable>
            <variable id=""^_orT3]zalQ%?q/~0:]/"">result</variable>
            <variable id=""_zee4C|@;2X{Y{olR-pn"">Hint</variable>
        </variables>
        <block type=""variables_set"" id=""JT46ucwLa)Xoz56MX(0:"" x=""75"" y=""25"">
            <field name=""VAR"" id=""ZDIXGT%Z(UJRi^~RieuB"">list</field>
            <value name=""VALUE"">
            <block type=""lists_create_with"" id=""vB++~W19HPA[7#/+xfv]"">
                <mutation items=""0""></mutation>
            </block>
            </value>
            <next>
            <block type=""controls_for"" id=""OMc.(TC4$T])}RWF5_(~"">
                <field name=""VAR"" id=""QkoPNb%d.!LDp_agPt.M"">i</field>
                <value name=""FROM"">
                <shadow type=""math_number"" id=""[m^?eC#+q=DXU.H%N0H-"">
                    <field name=""NUM"">1</field>
                </shadow>
                </value>
                <value name=""TO"">
                <shadow type=""math_number"" id=""JB#ojhlCK*8~#K!lS-hM"">
                    <field name=""NUM"">10</field>
                </shadow>
                </value>
                <value name=""BY"">
                <shadow type=""math_number"" id=""IU)@2W.*bfZKgp;c,[|]"">
                    <field name=""NUM"">1</field>
                </shadow>
                </value>
                <statement name=""DO"">
                <block type=""delay"" id=""agBwLazbhV]OqKBs;P5W"">
                    <field name=""DELAY"">Delay (ms)</field>
                    <value name=""DELAY"">
                    <shadow type=""math_number"" id=""8xm@!Own(_,$~eG93s4l"">
                        <field name=""NUM"">50</field>
                    </shadow>
                    </value>
                    <next>
                    <block type=""lists_setIndex"" id=""/:N_iDv3|JbFCX`Ac/+?"">
                        <mutation at=""false""></mutation>
                        <field name=""MODE"">INSERT</field>
                        <field name=""WHERE"">LAST</field>
                        <value name=""LIST"">
                        <block type=""variables_get"" id=""X:L[u%E+8DS?PbmdW:@@"">
                            <field name=""VAR"" id=""ZDIXGT%Z(UJRi^~RieuB"">list</field>
                        </block>
                        </value>
                        <value name=""TO"">
                        <block type=""variables_get"" id=""|0z,wnSn+T1vpz;0FXSY"">
                            <field name=""VAR"" id=""_zee4C|@;2X{Y{olR-pn"">Hint</field>
                        </block>
                        </value>
                    </block>
                    </next>
                </block>
                </statement>
                <next>
                <block type=""variables_set"" id=""d,w=A^5HnYk8^0bU=D`x"">
                    <field name=""VAR"" id=""^_orT3]zalQ%?q/~0:]/"">result</field>
                    <value name=""VALUE"">
                    <block type=""lists_split"" id=""TYtoRRUp|+-VEi,fePY$"">
                        <mutation mode=""JOIN""></mutation>
                        <field name=""MODE"">JOIN</field>
                        <value name=""INPUT"">
                        <block type=""variables_get"" id=""d:HA)*XZysEDzx]D8K}^"">
                            <field name=""VAR"" id=""ZDIXGT%Z(UJRi^~RieuB"">list</field>
                        </block>
                        </value>
                        <value name=""DELIM"">
                        <shadow type=""text"" id=""9NT)y?GJ_X[#uUM1Nv*g"">
                            <field name=""TEXT""> ** </field>
                        </shadow>
                        </value>
                    </block>
                    </value>
                </block>
                </next>
            </block>
            </next>
        </block>
    </xml>";

    public static string DebugScript5Outer(string innerId) => @"<xml xmlns=""https://developers.google.com/blockly/xml"">
        <variables>
            <variable id=""SG?L?f`u1D@r|U=pZf#4"">parallel</variable>
            <variable id=""Zmt}Hg1LuhcknI~,:o2$"">result</variable>
        </variables>
        <block type=""variables_set"" id=""lgk6.v@;_hO}aX0V}l*V"" x=""125"" y=""125"">
            <field name=""VAR"" id=""SG?L?f`u1D@r|U=pZf#4"">parallel</field>
            <value name=""VALUE"">
            <block type=""run_script_in_parallel"" id=""ty]qMj_s4k[XtNU31j0-"">
                <field name=""SCRIPTS"">scripts</field>
                <field name=""LEADINGSCRIPT"">leading</field>
                <value name=""SCRIPTS"">
                <block type=""lists_create_with"" id=""L##^|esHF$Q,*H,Vj(|7"">
                    <mutation items=""3""></mutation>
                    <value name=""ADD0"">
                    <block type=""run_script_by_name"" id=""!)S%-$%nBUzR!#-]x!P="">
                        <value name=""NAME"">
                        <shadow type=""text"" id=""O3F,ir/FqRk;Aa#.V5_Q"">
                            <field name=""TEXT""></field>
                        </shadow>
                        <block type=""text"" id=""f22^WNq#pPo7pW^MG)4T"">
                            <field name=""TEXT"">Jochen - Debug Demo 5 (Inner)</field>
                        </block>
                        </value>
                        <value name=""ID"">
                        <block type=""text"" id=""AEYW^H]-=nE=HJ#Q5*6}"">
                            <field name=""TEXT"">$$InnerId$$</field>
                        </block>
                        </value>
                        <value name=""ARGS"">
                        <block type=""lists_create_with"" id=""uoig`Uzntz6~$MX~L{2a"">
                            <mutation items=""1""></mutation>
                            <value name=""ADD0"">
                            <block type=""create_script_parameter"" id=""Ezs4^6$`Oy4Z(bZK)EB_"">
                                <field name=""NAME"">Variable name</field>
                                <field name=""VALUE"">Value</field>
                                <value name=""NAME"">
                                <block type=""text"" id=""Wj@{h=K,@P}B(,8[oLn~"">
                                    <field name=""TEXT"">Hint</field>
                                </block>
                                </value>
                                <value name=""VALUE"">
                                <block type=""text"" id=""h1xB86S`o}wUU9MF#C^8"">
                                    <field name=""TEXT"">A</field>
                                </block>
                                </value>
                            </block>
                            </value>
                        </block>
                        </value>
                        <value name=""BUILDONLY"">
                        <shadow type=""logic_boolean"" id=""eXT-!uufRp0!6NV7yJFK"">
                            <field name=""BOOL"">FALSE</field>
                        </shadow>
                        </value>
                    </block>
                    </value>
                    <value name=""ADD1"">
                    <block type=""run_script_by_name"" id=""d=x(+{E=MH+P}|7k~]D,"">
                        <value name=""NAME"">
                        <shadow type=""text"" id=""O3F,ir/FqRk;Aa#.V5_Q"">
                            <field name=""TEXT""></field>
                        </shadow>
                        <block type=""text"" id=""BHQ{jzIo.x~jPttX1wAm"">
                            <field name=""TEXT"">Jochen - Debug Demo 5 (Inner)</field>
                        </block>
                        </value>
                        <value name=""ID"">
                        <block type=""text"" id=""NIOq8r6=5JTvfun@vt;I"">
                            <field name=""TEXT"">$$InnerId$$</field>
                        </block>
                        </value>
                        <value name=""ARGS"">
                        <block type=""lists_create_with"" id=""I?h%fZ*n}ps.14i5gB$r"">
                            <mutation items=""1""></mutation>
                            <value name=""ADD0"">
                            <block type=""create_script_parameter"" id=""s{eY-IbF+DJ5C+ea|]^^"">
                                <field name=""NAME"">Variable name</field>
                                <field name=""VALUE"">Value</field>
                                <value name=""NAME"">
                                <block type=""text"" id=""F?.H8vJkh]1itx;z=8_i"">
                                    <field name=""TEXT"">Hint</field>
                                </block>
                                </value>
                                <value name=""VALUE"">
                                <block type=""text"" id=""dkshGerj,}tmvw)LWNuw"">
                                    <field name=""TEXT"">B</field>
                                </block>
                                </value>
                            </block>
                            </value>
                        </block>
                        </value>
                        <value name=""BUILDONLY"">
                        <shadow type=""logic_boolean"" id=""`J!OzG~53{50RqCpknFT"">
                            <field name=""BOOL"">FALSE</field>
                        </shadow>
                        </value>
                    </block>
                    </value>
                    <value name=""ADD2"">
                    <block type=""run_script_by_name"" id=""b,Qgwaf%rV0|z$zRUL@P"">
                        <value name=""NAME"">
                        <shadow type=""text"" id=""O3F,ir/FqRk;Aa#.V5_Q"">
                            <field name=""TEXT""></field>
                        </shadow>
                        <block type=""text"" id=""+;m)ukSt;PK%c8a=X!E`"">
                            <field name=""TEXT"">Jochen - Debug Demo 5 (Inner)</field>
                        </block>
                        </value>
                        <value name=""ID"">
                        <block type=""text"" id=""`gd{G:^,PaWSmVHy2Tt%"">
                            <field name=""TEXT"">$$InnerId$$</field>
                        </block>
                        </value>
                        <value name=""ARGS"">
                        <block type=""lists_create_with"" id=""YUf]9;oi=,:}qUUJQR:n"">
                            <mutation items=""1""></mutation>
                            <value name=""ADD0"">
                            <block type=""create_script_parameter"" id=""4rsM*r^KA(rrD-$e4,~W"">
                                <field name=""NAME"">Variable name</field>
                                <field name=""VALUE"">Value</field>
                                <value name=""NAME"">
                                <block type=""text"" id=""(+lVw@ZnNS]=BO0bgaLH"">
                                    <field name=""TEXT"">Hint</field>
                                </block>
                                </value>
                                <value name=""VALUE"">
                                <block type=""text"" id=""/uK?LjFvj`9bAj@kQVRa"">
                                    <field name=""TEXT"">C</field>
                                </block>
                                </value>
                            </block>
                            </value>
                        </block>
                        </value>
                        <value name=""BUILDONLY"">
                        <shadow type=""logic_boolean"" id=""3Sf]hfku,TmWrqfbS}qr"">
                            <field name=""BOOL"">FALSE</field>
                        </shadow>
                        </value>
                    </block>
                    </value>
                </block>
                </value>
            </block>
            </value>
            <next>
            <block type=""variables_set"" id=""oN]Q)P-|fWu2?kZJ%IE0"">
                <field name=""VAR"" id=""Zmt}Hg1LuhcknI~,:o2$"">result</field>
                <value name=""VALUE"">
                <block type=""lists_split"" id=""~(tf[l*yUlbT/;(u~fze"">
                    <mutation mode=""JOIN""></mutation>
                    <field name=""MODE"">JOIN</field>
                    <value name=""INPUT"">
                    <block type=""variables_get"" id=""rUE32}]/L9JtVl%MpYcT"">
                        <field name=""VAR"" id=""SG?L?f`u1D@r|U=pZf#4"">parallel</field>
                    </block>
                    </value>
                    <value name=""DELIM"">
                    <shadow type=""text"" id=""2`zo(7HqL^-^EC=`Nwl,"">
                        <field name=""TEXT""> ++ </field>
                    </shadow>
                    </value>
                </block>
                </value>
            </block>
            </next>
        </block>
    </xml>".Replace("$$InnerId$$", innerId);
}