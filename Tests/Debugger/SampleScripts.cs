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
}