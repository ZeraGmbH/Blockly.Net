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
}