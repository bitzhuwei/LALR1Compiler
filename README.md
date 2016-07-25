# LALR1Compiler
generate LR(0), SLR, LALR(1) and LR(1) compiler code(C#) for given grammar.
<p style="text-align: center;"><span style="font-size: 16pt;"><strong>基于虎书实现LALR(1)分析并生成GLSL编译器前端代码(C#) </strong></span></p>
<p>为了完美解析GLSL源码，获取其中的信息（都有哪些in/out/uniform等），我决定做个GLSL编译器的前端（以后简称编译器或FrontEndParser）。</p>
<p>以前我做过一个CGCompiler，可以自动生成LL(1)文法的编译器代码(C#语言的)。于是我从《The OpenGL &reg; Shading Language》（以下简称"PDF"）找到一个GLSL的文法，就开始试图将他改写为LL(1)文法。等到我重写了7次后发现，这是不可能的。GLSL的文法超出了LL(1)的范围，必须用更强的分析算法。于是有了现在的<a href="https://github.com/bitzhuwei/LALR1Compiler/" target="_blank">LALR(1)Compiler</a>。</p>
<h1>理论来源</h1>
<p>《现代编译原理-c语言描述》（即"<span style="color: red;">虎书</span>"）中提供了详尽的资料。我就以虎书为理论依据。</p>
<p>虎书中的下图表明了各种类型的文法的范围。一般正常的程序语言都是符合LALR(1)文法的。</p>
<p>由于LR(0)是SLR的基础，SLR是LR(1)的基础；又由于LR(1)是LALR(1)的基础（这看上去有点奇怪），所以我必须从LR(0)文法开始一步一步实现<span style="color: red;">LALR(1)算法</span>。</p>
<p><img src="http://images2015.cnblogs.com/blog/383191/201604/383191-20160415230927473-1393272012.png" alt="" /></p>
<h1>输入</h1>
<p>给定文法，这个文法所描述的语言的全部信息就都包含进去了。文法里包含了这个语言的关键字、推导结构等所有信息。这也是我觉得YACC那些东西不好的地方：明明有了文法，还得自己整理出各种关键字。</p>
<p>下面是一个文法的例子：</p>
<div class="cnblogs_code">
<pre><span style="color: #008080;">1</span> <span style="color: #008000;">//</span><span style="color: #008000;"> 虎书中的文法3-10</span>
<span style="color: #008080;">2</span> &lt;S&gt; ::= &lt;V&gt; <span style="color: #800000;">"</span><span style="color: #800000;">=</span><span style="color: #800000;">"</span> &lt;E&gt;<span style="color: #000000;"> ;
</span><span style="color: #008080;">3</span> &lt;S&gt; ::= &lt;E&gt;<span style="color: #000000;"> ;
</span><span style="color: #008080;">4</span> &lt;E&gt; ::= &lt;V&gt;<span style="color: #000000;"> ;
</span><span style="color: #008080;">5</span> &lt;V&gt; ::= <span style="color: #800000;">"</span><span style="color: #800000;">x</span><span style="color: #800000;">"</span><span style="color: #000000;"> ;
</span><span style="color: #008080;">6</span> &lt;V&gt; ::= <span style="color: #800000;">"</span><span style="color: #800000;">*</span><span style="color: #800000;">"</span> &lt;E&gt; ;</pre>
</div>
<div>&nbsp;</div>
<p>下面是6个符合此文法的代码：</p>
<div class="cnblogs_code">
<pre><span style="color: #008080;">1</span> <span style="color: #000000;">x
</span><span style="color: #008080;">2</span> *<span style="color: #000000;">x
</span><span style="color: #008080;">3</span> x =<span style="color: #000000;"> x
</span><span style="color: #008080;">4</span> x = *<span style="color: #000000;"> x
</span><span style="color: #008080;">5</span> *x =<span style="color: #000000;"> x
</span><span style="color: #008080;">6</span> **x = **x</pre>
</div>
<h1>输出</h1>
<p>输出结果是此文法的<span style="color: red;">编译器代码(C#)</span>。这主要是词法分析器LexicalAnalyzer和语法分析器SyntaxParser两个类。</p>
<p>之后利用C#的<span style="color: #2b91af; font-family: 新宋体; background-color: white;">CSharpCodeProvider</span>和反射技术来加载、编译、运行生成的代码，用一些例子（例如上面的*x = x）测试是否能正常运行。只要能正常生成语法树，就证明了我的LALR(1)Compiler的实现是正确的。</p>
<p>例如对上述文法的6个示例代码，LALR(1)Compiler可以分别dump出如下的语法树：</p>
<div class="cnblogs_code" onclick="cnblogs_code_show('c5402fb6-08b5-4e7b-b6b3-c498206883b8')"><img id="code_img_closed_c5402fb6-08b5-4e7b-b6b3-c498206883b8" class="code_img_closed" src="http://images.cnblogs.com/OutliningIndicators/ContractedBlock.gif" alt="" /><img id="code_img_opened_c5402fb6-08b5-4e7b-b6b3-c498206883b8" class="code_img_opened" style="display: none;" onclick="cnblogs_code_hide('c5402fb6-08b5-4e7b-b6b3-c498206883b8',event)" src="http://images.cnblogs.com/OutliningIndicators/ExpandedBlockStart.gif" alt="" />
<div id="cnblogs_code_open_c5402fb6-08b5-4e7b-b6b3-c498206883b8" class="cnblogs_code_hide">
<pre><span style="color: #008080;">1</span> (__S)[S][&lt;S&gt;<span style="color: #000000;">]
</span><span style="color: #008080;">2</span>  └─(__E)[E][&lt;E&gt;<span style="color: #000000;">]
</span><span style="color: #008080;">3</span>      └─(__V)[V][&lt;V&gt;<span style="color: #000000;">]
</span><span style="color: #008080;">4</span>          └─(__xLeave__)[x][x]</pre>
</div>
<span class="cnblogs_code_collapse">x</span></div>
<div class="cnblogs_code" onclick="cnblogs_code_show('7df7ed1d-be82-4760-9422-032a65b748d9')"><img id="code_img_closed_7df7ed1d-be82-4760-9422-032a65b748d9" class="code_img_closed" src="http://images.cnblogs.com/OutliningIndicators/ContractedBlock.gif" alt="" /><img id="code_img_opened_7df7ed1d-be82-4760-9422-032a65b748d9" class="code_img_opened" style="display: none;" onclick="cnblogs_code_hide('7df7ed1d-be82-4760-9422-032a65b748d9',event)" src="http://images.cnblogs.com/OutliningIndicators/ExpandedBlockStart.gif" alt="" />
<div id="cnblogs_code_open_7df7ed1d-be82-4760-9422-032a65b748d9" class="cnblogs_code_hide">
<pre><span style="color: #008080;">1</span> (__S)[S][&lt;S&gt;<span style="color: #000000;">]
</span><span style="color: #008080;">2</span>  └─(__E)[E][&lt;E&gt;<span style="color: #000000;">]
</span><span style="color: #008080;">3</span>      └─(__V)[V][&lt;V&gt;<span style="color: #000000;">]
</span><span style="color: #008080;">4</span>          ├─(__starLeave__)[*][<span style="color: #800000;">"</span><span style="color: #800000;">*</span><span style="color: #800000;">"</span><span style="color: #000000;">]
</span><span style="color: #008080;">5</span>          └─(__E)[E][&lt;E&gt;<span style="color: #000000;">]
</span><span style="color: #008080;">6</span>              └─(__V)[V][&lt;V&gt;<span style="color: #000000;">]
</span><span style="color: #008080;">7</span>                  └─(__xLeave__)[x][x]</pre>
</div>
<span class="cnblogs_code_collapse">*x</span></div>
<div class="cnblogs_code" onclick="cnblogs_code_show('568cec47-0a49-42f7-9843-1404bf02ddca')"><img id="code_img_closed_568cec47-0a49-42f7-9843-1404bf02ddca" class="code_img_closed" src="http://images.cnblogs.com/OutliningIndicators/ContractedBlock.gif" alt="" /><img id="code_img_opened_568cec47-0a49-42f7-9843-1404bf02ddca" class="code_img_opened" style="display: none;" onclick="cnblogs_code_hide('568cec47-0a49-42f7-9843-1404bf02ddca',event)" src="http://images.cnblogs.com/OutliningIndicators/ExpandedBlockStart.gif" alt="" />
<div id="cnblogs_code_open_568cec47-0a49-42f7-9843-1404bf02ddca" class="cnblogs_code_hide">
<pre><span style="color: #008080;">1</span> (__S)[S][&lt;S&gt;<span style="color: #000000;">]
</span><span style="color: #008080;">2</span>  ├─(__V)[V][&lt;V&gt;<span style="color: #000000;">]
</span><span style="color: #008080;">3</span> <span style="color: #000000;"> │  └─(__xLeave__)[x][x]
</span><span style="color: #008080;">4</span>  ├─(__equalLeave__)[=][<span style="color: #800000;">"</span><span style="color: #800000;">=</span><span style="color: #800000;">"</span><span style="color: #000000;">]
</span><span style="color: #008080;">5</span>  └─(__E)[E][&lt;E&gt;<span style="color: #000000;">]
</span><span style="color: #008080;">6</span>      └─(__V)[V][&lt;V&gt;<span style="color: #000000;">]
</span><span style="color: #008080;">7</span>          └─(__xLeave__)[x][x]</pre>
</div>
<span class="cnblogs_code_collapse">x = x</span></div>
<div class="cnblogs_code" onclick="cnblogs_code_show('58dff601-0f6b-4040-b9b2-aa5418048514')"><img id="code_img_closed_58dff601-0f6b-4040-b9b2-aa5418048514" class="code_img_closed" src="http://images.cnblogs.com/OutliningIndicators/ContractedBlock.gif" alt="" /><img id="code_img_opened_58dff601-0f6b-4040-b9b2-aa5418048514" class="code_img_opened" style="display: none;" onclick="cnblogs_code_hide('58dff601-0f6b-4040-b9b2-aa5418048514',event)" src="http://images.cnblogs.com/OutliningIndicators/ExpandedBlockStart.gif" alt="" />
<div id="cnblogs_code_open_58dff601-0f6b-4040-b9b2-aa5418048514" class="cnblogs_code_hide">
<pre><span style="color: #008080;"> 1</span> (__S)[S][&lt;S&gt;<span style="color: #000000;">]
</span><span style="color: #008080;"> 2</span>  ├─(__V)[V][&lt;V&gt;<span style="color: #000000;">]
</span><span style="color: #008080;"> 3</span> <span style="color: #000000;"> │  └─(__xLeave__)[x][x]
</span><span style="color: #008080;"> 4</span>  ├─(__equalLeave__)[=][<span style="color: #800000;">"</span><span style="color: #800000;">=</span><span style="color: #800000;">"</span><span style="color: #000000;">]
</span><span style="color: #008080;"> 5</span>  └─(__E)[E][&lt;E&gt;<span style="color: #000000;">]
</span><span style="color: #008080;"> 6</span>      └─(__V)[V][&lt;V&gt;<span style="color: #000000;">]
</span><span style="color: #008080;"> 7</span>          ├─(__starLeave__)[*][<span style="color: #800000;">"</span><span style="color: #800000;">*</span><span style="color: #800000;">"</span><span style="color: #000000;">]
</span><span style="color: #008080;"> 8</span>          └─(__E)[E][&lt;E&gt;<span style="color: #000000;">]
</span><span style="color: #008080;"> 9</span>              └─(__V)[V][&lt;V&gt;<span style="color: #000000;">]
</span><span style="color: #008080;">10</span>                  └─(__xLeave__)[x][x]</pre>
</div>
<span class="cnblogs_code_collapse">x = * x</span></div>
<div class="cnblogs_code" onclick="cnblogs_code_show('5e6f7b0f-7edf-4df4-92ea-a16fcf7e2975')"><img id="code_img_closed_5e6f7b0f-7edf-4df4-92ea-a16fcf7e2975" class="code_img_closed" src="http://images.cnblogs.com/OutliningIndicators/ContractedBlock.gif" alt="" /><img id="code_img_opened_5e6f7b0f-7edf-4df4-92ea-a16fcf7e2975" class="code_img_opened" style="display: none;" onclick="cnblogs_code_hide('5e6f7b0f-7edf-4df4-92ea-a16fcf7e2975',event)" src="http://images.cnblogs.com/OutliningIndicators/ExpandedBlockStart.gif" alt="" />
<div id="cnblogs_code_open_5e6f7b0f-7edf-4df4-92ea-a16fcf7e2975" class="cnblogs_code_hide">
<pre><span style="color: #008080;"> 1</span> (__S)[S][&lt;S&gt;<span style="color: #000000;">]
</span><span style="color: #008080;"> 2</span>  ├─(__V)[V][&lt;V&gt;<span style="color: #000000;">]
</span><span style="color: #008080;"> 3</span>  │  ├─(__starLeave__)[*][<span style="color: #800000;">"</span><span style="color: #800000;">*</span><span style="color: #800000;">"</span><span style="color: #000000;">]
</span><span style="color: #008080;"> 4</span>  │  └─(__E)[E][&lt;E&gt;<span style="color: #000000;">]
</span><span style="color: #008080;"> 5</span>  │      └─(__V)[V][&lt;V&gt;<span style="color: #000000;">]
</span><span style="color: #008080;"> 6</span> <span style="color: #000000;"> │          └─(__xLeave__)[x][x]
</span><span style="color: #008080;"> 7</span>  ├─(__equalLeave__)[=][<span style="color: #800000;">"</span><span style="color: #800000;">=</span><span style="color: #800000;">"</span><span style="color: #000000;">]
</span><span style="color: #008080;"> 8</span>  └─(__E)[E][&lt;E&gt;<span style="color: #000000;">]
</span><span style="color: #008080;"> 9</span>      └─(__V)[V][&lt;V&gt;<span style="color: #000000;">]
</span><span style="color: #008080;">10</span>          └─(__xLeave__)[x][x]</pre>
</div>
<span class="cnblogs_code_collapse">*x = x</span></div>
<div class="cnblogs_code" onclick="cnblogs_code_show('e0deee34-95bf-4855-ae15-c4735506096e')"><img id="code_img_closed_e0deee34-95bf-4855-ae15-c4735506096e" class="code_img_closed" src="http://images.cnblogs.com/OutliningIndicators/ContractedBlock.gif" alt="" /><img id="code_img_opened_e0deee34-95bf-4855-ae15-c4735506096e" class="code_img_opened" style="display: none;" onclick="cnblogs_code_hide('e0deee34-95bf-4855-ae15-c4735506096e',event)" src="http://images.cnblogs.com/OutliningIndicators/ExpandedBlockStart.gif" alt="" />
<div id="cnblogs_code_open_e0deee34-95bf-4855-ae15-c4735506096e" class="cnblogs_code_hide">
<pre><span style="color: #008080;"> 1</span> (__S)[S][&lt;S&gt;<span style="color: #000000;">]
</span><span style="color: #008080;"> 2</span>  ├─(__V)[V][&lt;V&gt;<span style="color: #000000;">]
</span><span style="color: #008080;"> 3</span>  │  ├─(__starLeave__)[*][<span style="color: #800000;">"</span><span style="color: #800000;">*</span><span style="color: #800000;">"</span><span style="color: #000000;">]
</span><span style="color: #008080;"> 4</span>  │  └─(__E)[E][&lt;E&gt;<span style="color: #000000;">]
</span><span style="color: #008080;"> 5</span>  │      └─(__V)[V][&lt;V&gt;<span style="color: #000000;">]
</span><span style="color: #008080;"> 6</span>  │          ├─(__starLeave__)[*][<span style="color: #800000;">"</span><span style="color: #800000;">*</span><span style="color: #800000;">"</span><span style="color: #000000;">]
</span><span style="color: #008080;"> 7</span>  │          └─(__E)[E][&lt;E&gt;<span style="color: #000000;">]
</span><span style="color: #008080;"> 8</span>  │              └─(__V)[V][&lt;V&gt;<span style="color: #000000;">]
</span><span style="color: #008080;"> 9</span> <span style="color: #000000;"> │                  └─(__xLeave__)[x][x]
</span><span style="color: #008080;">10</span>  ├─(__equalLeave__)[=][<span style="color: #800000;">"</span><span style="color: #800000;">=</span><span style="color: #800000;">"</span><span style="color: #000000;">]
</span><span style="color: #008080;">11</span>  └─(__E)[E][&lt;E&gt;<span style="color: #000000;">]
</span><span style="color: #008080;">12</span>      └─(__V)[V][&lt;V&gt;<span style="color: #000000;">]
</span><span style="color: #008080;">13</span>          ├─(__starLeave__)[*][<span style="color: #800000;">"</span><span style="color: #800000;">*</span><span style="color: #800000;">"</span><span style="color: #000000;">]
</span><span style="color: #008080;">14</span>          └─(__E)[E][&lt;E&gt;<span style="color: #000000;">]
</span><span style="color: #008080;">15</span>              └─(__V)[V][&lt;V&gt;<span style="color: #000000;">]
</span><span style="color: #008080;">16</span>                  ├─(__starLeave__)[*][<span style="color: #800000;">"</span><span style="color: #800000;">*</span><span style="color: #800000;">"</span><span style="color: #000000;">]
</span><span style="color: #008080;">17</span>                  └─(__E)[E][&lt;E&gt;<span style="color: #000000;">]
</span><span style="color: #008080;">18</span>                      └─(__V)[V][&lt;V&gt;<span style="color: #000000;">]
</span><span style="color: #008080;">19</span>                          └─(__xLeave__)[x][x]</pre>
</div>
<span class="cnblogs_code_collapse">**x = **x</span></div>
<p><span style="line-height: 1.5;">能够正确地导出这些结果，就说明整个库是正确的。其实，只要能导出这些结果而不throw Exception()，就可以断定结果是正确的了</span></p>
<h1>计划</h1>
<p>所以我的开发步骤如下：</p>
<h2>示例</h2>
<p>虎书中已有了文法3-1（如下）的分析表和一个示例分析过程，所以先手工实现文法3-1的分析器。从这个分析器的代码中抽取出所有LR分析器<span style="color: red;">通用的部分</span>，作为LALR(1)Compiler的一部分。</p>
<div class="cnblogs_code">
<pre><span style="color: #008080;"> 1</span> <span style="color: #008000;">//</span><span style="color: #008000;"> 虎书中的文法3-1</span>
<span style="color: #008080;"> 2</span> &lt;S&gt; ::= &lt;S&gt; <span style="color: #800000;">"</span><span style="color: #800000;">;</span><span style="color: #800000;">"</span> &lt;S&gt;<span style="color: #000000;"> ;
</span><span style="color: #008080;"> 3</span> &lt;S&gt; ::= identifier <span style="color: #800000;">"</span><span style="color: #800000;">:=</span><span style="color: #800000;">"</span> &lt;E&gt;<span style="color: #000000;"> ;
</span><span style="color: #008080;"> 4</span> &lt;S&gt; ::= <span style="color: #800000;">"</span><span style="color: #800000;">print</span><span style="color: #800000;">"</span> <span style="color: #800000;">"</span><span style="color: #800000;">(</span><span style="color: #800000;">"</span> &lt;L&gt; <span style="color: #800000;">"</span><span style="color: #800000;">)</span><span style="color: #800000;">"</span><span style="color: #000000;"> ;
</span><span style="color: #008080;"> 5</span> &lt;E&gt; ::=<span style="color: #000000;"> identifier ;
</span><span style="color: #008080;"> 6</span> &lt;E&gt; ::=<span style="color: #000000;"> number ;
</span><span style="color: #008080;"> 7</span> &lt;E&gt; ::= &lt;E&gt; <span style="color: #800000;">"</span><span style="color: #800000;">+</span><span style="color: #800000;">"</span> &lt;E&gt;<span style="color: #000000;"> ;
</span><span style="color: #008080;"> 8</span> &lt;E&gt; ::= <span style="color: #800000;">"</span><span style="color: #800000;">(</span><span style="color: #800000;">"</span> &lt;S&gt; <span style="color: #800000;">"</span><span style="color: #800000;">,</span><span style="color: #800000;">"</span> &lt;E&gt; <span style="color: #800000;">"</span><span style="color: #800000;">)</span><span style="color: #800000;">"</span><span style="color: #000000;"> ;
</span><span style="color: #008080;"> 9</span> &lt;L&gt; ::= &lt;E&gt;<span style="color: #000000;"> ;
</span><span style="color: #008080;">10</span> &lt;L&gt; ::= &lt;L&gt; <span style="color: #800000;">"</span><span style="color: #800000;">,</span><span style="color: #800000;">"</span> &lt;E&gt; ;</pre>
</div>
<h2>算法</h2>
<p>经此之后就对语法分析器的构成心中有数了。下面实现虎书中关于<span style="color: red;">自动生成工具</span>的算法。</p>
<p>最妙的是，即使开始时不理解这些算法的原理，也能够实现之。实现后通过测试用例debug的过程，就很容易理解这些算法了。</p>
<h3>LR(0)</h3>
<p>首先有两个基础算法。<span style="color: red;">Closure</span>用于补全一个state。<span style="color: red;">Goto</span>用于找到一个state经过某个Node后会进入的下一个state。说是算法，其实却非常简单。虽然简单，要想实现却有很多额外的工作。例如比较两个LR(0)Item的问题。</p>
<p><img src="http://images2015.cnblogs.com/blog/383191/201604/383191-20160415230928645-1184612160.png" alt="" /></p>
<p>然后就是计算文法的<span style="color: red;">状态集</span>和<span style="color: red;">边集</span>（Goto动作集）的算法。这个是核心内容。</p>
<p><img src="http://images2015.cnblogs.com/blog/383191/201604/383191-20160415230929441-1747615526.png" alt="" /></p>
<p>用此算法可以画出文法3-8的状态图如下：</p>
<div class="cnblogs_code">
<pre><span style="color: #008080;">1</span> <span style="color: #008000;">//</span><span style="color: #008000;"> 虎书中的文法3-8</span>
<span style="color: #008080;">2</span> &lt;S&gt; ::= <span style="color: #800000;">"</span><span style="color: #800000;">(</span><span style="color: #800000;">"</span> &lt;L&gt; <span style="color: #800000;">"</span><span style="color: #800000;">)</span><span style="color: #800000;">"</span><span style="color: #000000;"> ;
</span><span style="color: #008080;">3</span> &lt;S&gt; ::= <span style="color: #800000;">"</span><span style="color: #800000;">x</span><span style="color: #800000;">"</span><span style="color: #000000;"> ;
</span><span style="color: #008080;">4</span> &lt;L&gt; ::= &lt;S&gt;<span style="color: #000000;"> ;
</span><span style="color: #008080;">5</span> &lt;L&gt; ::= &lt;L&gt; <span style="color: #800000;">"</span><span style="color: #800000;">,</span><span style="color: #800000;">"</span> &lt;S&gt; ;</pre>
</div>
<p><img src="http://images2015.cnblogs.com/blog/383191/201604/383191-20160415230930363-769867346.png" alt="" /></p>
<p>最后就是看图作文&mdash;&mdash;构造分析表了。有了分析表，语法分析器的核心部分就完成了。</p>
<p><img src="http://images2015.cnblogs.com/blog/383191/201604/383191-20160415230931238-901875708.png" alt="" /></p>
<p>&nbsp;</p>
<h3>SLR</h3>
<p>在A-&gt;&alpha;.可以被归约时，只在下一个单词是Follow(A)时才进行归约。看起来很有道理的样子。</p>
<p><img src="http://images2015.cnblogs.com/blog/383191/201604/383191-20160415230932332-1237441662.png" alt="" /></p>
<h3>LR(1)</h3>
<p>LR(1)项（A-&gt;&alpha;.&beta;,x）指出，序列&alpha;在栈顶，且输入中开头的是可以从&beta;x导出的符号。看起来更有道理的样子。</p>
<p><img src="http://images2015.cnblogs.com/blog/383191/201604/383191-20160415230933223-2008085940.png" alt="" /></p>
<p>LR(1)的state补全和转换算法也要调整。</p>
<p><img src="http://images2015.cnblogs.com/blog/383191/201604/383191-20160415230934863-1694160325.png" alt="" /></p>
<p>然后又是看图作文。</p>
<p><img src="http://images2015.cnblogs.com/blog/383191/201604/383191-20160415230936207-701094906.png" alt="" /></p>
<p>&nbsp;</p>
<h3>LALR(1)</h3>
<p>LALR(1)是对LA(1)的化简处理。他占用空间比LR(1)少，但应用范围也比LR(1)小了点。</p>
<p><img src="http://images2015.cnblogs.com/blog/383191/201604/383191-20160415230936895-1524638639.png" alt="" /></p>
<p>为了实现LALR(1)，也为了提高LR(1)的效率，必须优化LR(1)State，不能再单纯模仿LR(0)State了。</p>
<h2>文法的文法</h2>
<p>输入的是文法，输出的是编译器代码，这个过程也可以用一个编译器来实现。这个特别的编译器所对应的文法（即<span style="color: red;">描述文法的文法</span>）如下：（此编译器命名为ContextfreeGrammarCompiler）</p>
<div class="cnblogs_code">
<pre><span style="color: #008080;"> 1</span> <span style="color: #008000;">//</span><span style="color: #008000;"> 文法是1到多个产生式</span>
<span style="color: #008080;"> 2</span> &lt;Grammar&gt; ::= &lt;ProductionList&gt; &lt;Production&gt;<span style="color: #000000;"> ;
</span><span style="color: #008080;"> 3</span> <span style="color: #008000;">//</span><span style="color: #008000;"> 产生式列表是0到多个产生式</span>
<span style="color: #008080;"> 4</span> &lt;ProductionList&gt; ::= &lt;ProductionList&gt; &lt;Production&gt; | <span style="color: #0000ff;">null</span><span style="color: #000000;"> ;
</span><span style="color: #008080;"> 5</span> <span style="color: #008000;">//</span><span style="color: #008000;"> 产生式是左部+第一个候选式+若干右部</span>
<span style="color: #008080;"> 6</span> &lt;Production&gt; ::= &lt;Vn&gt; <span style="color: #800000;">"</span><span style="color: #800000;">::=</span><span style="color: #800000;">"</span> &lt;Canditate&gt; &lt;RightPartList&gt; <span style="color: #800000;">"</span><span style="color: #800000;">;</span><span style="color: #800000;">"</span><span style="color: #000000;"> ;
</span><span style="color: #008080;"> 7</span> <span style="color: #008000;">//</span><span style="color: #008000;"> 候选式是1到多个结点</span>
<span style="color: #008080;"> 8</span> &lt;Canditate&gt; ::= &lt;VList&gt; &lt;V&gt;<span style="color: #000000;"> ;
</span><span style="color: #008080;"> 9</span> <span style="color: #008000;">//</span><span style="color: #008000;"> 结点列表是0到多个结点</span>
<span style="color: #008080;">10</span> &lt;VList&gt; ::= &lt;VList&gt; &lt;V&gt; | <span style="color: #0000ff;">null</span><span style="color: #000000;"> ;
</span><span style="color: #008080;">11</span> <span style="color: #008000;">//</span><span style="color: #008000;"> 右部列表是0到多个候选式</span>
<span style="color: #008080;">12</span> &lt;RightPartList&gt; ::= <span style="color: #800000;">"</span><span style="color: #800000;">|</span><span style="color: #800000;">"</span> &lt;Canditate&gt; &lt;RightPartList&gt; | <span style="color: #0000ff;">null</span><span style="color: #000000;"> ;
</span><span style="color: #008080;">13</span> <span style="color: #008000;">//</span><span style="color: #008000;"> 结点是非叶结点或叶结点</span>
<span style="color: #008080;">14</span> &lt;V&gt; ::= &lt;Vn&gt; | &lt;Vt&gt;<span style="color: #000000;"> ;
</span><span style="color: #008080;">15</span> <span style="color: #008000;">//</span><span style="color: #008000;"> 非叶结点是&lt;&gt;括起来的标识符</span>
<span style="color: #008080;">16</span> &lt;Vn&gt; ::= <span style="color: #800000;">"</span><span style="color: #800000;">&lt;</span><span style="color: #800000;">"</span> identifier <span style="color: #800000;">"</span><span style="color: #800000;">&gt;</span><span style="color: #800000;">"</span><span style="color: #000000;"> ;
</span><span style="color: #008080;">17</span> <span style="color: #008000;">//</span><span style="color: #008000;"> 叶结点是用"引起来的字符串常量或下列内容：null, identifier, number, constString, userDefinedType
</span><span style="color: #008080;">18</span> <span style="color: #008000;">//</span><span style="color: #008000;"> 这几个标识符就是ContextfreeGrammar的关键字</span>
<span style="color: #008080;">19</span> &lt;Vt&gt; ::= <span style="color: #800000;">"</span><span style="color: #800000;">null</span><span style="color: #800000;">"</span> | <span style="color: #800000;">"</span><span style="color: #800000;">identifier</span><span style="color: #800000;">"</span> | <span style="color: #800000;">"</span><span style="color: #800000;">number</span><span style="color: #800000;">"</span> | <span style="color: #800000;">"</span><span style="color: #800000;">constString</span><span style="color: #800000;">"</span> | <span style="color: #800000;">"</span><span style="color: #800000;">userDefinedType</span><span style="color: #800000;">"</span>| constString ;</pre>
</div>
<h1>设计</h1>
<p>算法看起来还是很简单的。即使不理解他也能实现他。但是实现过程中还是出现了不少的问题。</p>
<h2>Hash缓存</h2>
<p>如何判定两个对象（LR(0)Item）相同？</p>
<p>这是个不可小觑的问题。</p>
<p>必须重写==、!=运算符，override掉Equals和GetHashCode方法。这样才能判定两个内容相同但不是同一个对象的Item、State相等。</p>
<p>对于LR(0)Item的比较，在计算过程中有太多次，这对于实际应用（例如GLSL的文法）是不可接受的。所以必须缓存这类对象的HashCode。</p>
<div class="cnblogs_code" onclick="cnblogs_code_show('3eb2b45b-39da-41c0-b87a-125066ea0ea6')"><img id="code_img_closed_3eb2b45b-39da-41c0-b87a-125066ea0ea6" class="code_img_closed" src="http://images.cnblogs.com/OutliningIndicators/ContractedBlock.gif" alt="" /><img id="code_img_opened_3eb2b45b-39da-41c0-b87a-125066ea0ea6" class="code_img_opened" style="display: none;" onclick="cnblogs_code_hide('3eb2b45b-39da-41c0-b87a-125066ea0ea6',event)" src="http://images.cnblogs.com/OutliningIndicators/ExpandedBlockStart.gif" alt="" />
<div id="cnblogs_code_open_3eb2b45b-39da-41c0-b87a-125066ea0ea6" class="cnblogs_code_hide">
<pre><span style="color: #008080;">  1</span>     <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;summary&gt;</span>
<span style="color: #008080;">  2</span>     <span style="color: #808080;">///</span><span style="color: #008000;"> 缓存一个对象的hash code。提高比较（==、!=、Equals、GetHashCode、Compare）的效率。
</span><span style="color: #008080;">  3</span>     <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;/summary&gt;</span>
<span style="color: #008080;">  4</span>     <span style="color: #0000ff;">public</span> <span style="color: #0000ff;">abstract</span> <span style="color: #0000ff;">class</span> HashCache : IComparable&lt;HashCache&gt;
<span style="color: #008080;">  5</span> <span style="color: #000000;">    {
</span><span style="color: #008080;">  6</span>         <span style="color: #0000ff;">public</span> <span style="color: #0000ff;">static</span> <span style="color: #0000ff;">bool</span> <span style="color: #0000ff;">operator</span> ==<span style="color: #000000;">(HashCache left, HashCache right)
</span><span style="color: #008080;">  7</span> <span style="color: #000000;">        {
</span><span style="color: #008080;">  8</span>             <span style="color: #0000ff;">object</span> leftObj = left, rightObj =<span style="color: #000000;"> right;
</span><span style="color: #008080;">  9</span>             <span style="color: #0000ff;">if</span> (leftObj == <span style="color: #0000ff;">null</span><span style="color: #000000;">)
</span><span style="color: #008080;"> 10</span> <span style="color: #000000;">            {
</span><span style="color: #008080;"> 11</span>                 <span style="color: #0000ff;">if</span> (rightObj == <span style="color: #0000ff;">null</span>) { <span style="color: #0000ff;">return</span> <span style="color: #0000ff;">true</span><span style="color: #000000;">; }
</span><span style="color: #008080;"> 12</span>                 <span style="color: #0000ff;">else</span> { <span style="color: #0000ff;">return</span> <span style="color: #0000ff;">false</span><span style="color: #000000;">; }
</span><span style="color: #008080;"> 13</span> <span style="color: #000000;">            }
</span><span style="color: #008080;"> 14</span>             <span style="color: #0000ff;">else</span>
<span style="color: #008080;"> 15</span> <span style="color: #000000;">            {
</span><span style="color: #008080;"> 16</span>                 <span style="color: #0000ff;">if</span> (rightObj == <span style="color: #0000ff;">null</span>) { <span style="color: #0000ff;">return</span> <span style="color: #0000ff;">false</span><span style="color: #000000;">; }
</span><span style="color: #008080;"> 17</span> <span style="color: #000000;">            }
</span><span style="color: #008080;"> 18</span> 
<span style="color: #008080;"> 19</span>             <span style="color: #0000ff;">return</span><span style="color: #000000;"> left.Equals(right);
</span><span style="color: #008080;"> 20</span> <span style="color: #000000;">        }
</span><span style="color: #008080;"> 21</span> 
<span style="color: #008080;"> 22</span>         <span style="color: #0000ff;">public</span> <span style="color: #0000ff;">static</span> <span style="color: #0000ff;">bool</span> <span style="color: #0000ff;">operator</span> !=<span style="color: #000000;">(HashCache left, HashCache right)
</span><span style="color: #008080;"> 23</span> <span style="color: #000000;">        {
</span><span style="color: #008080;"> 24</span>             <span style="color: #0000ff;">return</span> !(left ==<span style="color: #000000;"> right);
</span><span style="color: #008080;"> 25</span> <span style="color: #000000;">        }
</span><span style="color: #008080;"> 26</span> 
<span style="color: #008080;"> 27</span>         <span style="color: #0000ff;">public</span> <span style="color: #0000ff;">override</span> <span style="color: #0000ff;">bool</span> Equals(<span style="color: #0000ff;">object</span><span style="color: #000000;"> obj)
</span><span style="color: #008080;"> 28</span> <span style="color: #000000;">        {
</span><span style="color: #008080;"> 29</span>             HashCache p = obj <span style="color: #0000ff;">as</span><span style="color: #000000;"> HashCache;
</span><span style="color: #008080;"> 30</span>             <span style="color: #0000ff;">if</span> ((System.Object)p == <span style="color: #0000ff;">null</span><span style="color: #000000;">)
</span><span style="color: #008080;"> 31</span> <span style="color: #000000;">            {
</span><span style="color: #008080;"> 32</span>                 <span style="color: #0000ff;">return</span> <span style="color: #0000ff;">false</span><span style="color: #000000;">;
</span><span style="color: #008080;"> 33</span> <span style="color: #000000;">            }
</span><span style="color: #008080;"> 34</span> 
<span style="color: #008080;"> 35</span>             <span style="color: #0000ff;">return</span> <span style="color: #0000ff;">this</span>.HashCode ==<span style="color: #000000;"> p.HashCode;
</span><span style="color: #008080;"> 36</span> <span style="color: #000000;">        }
</span><span style="color: #008080;"> 37</span> 
<span style="color: #008080;"> 38</span>         <span style="color: #0000ff;">public</span> <span style="color: #0000ff;">override</span> <span style="color: #0000ff;">int</span><span style="color: #000000;"> GetHashCode()
</span><span style="color: #008080;"> 39</span> <span style="color: #000000;">        {
</span><span style="color: #008080;"> 40</span>             <span style="color: #0000ff;">return</span> <span style="color: #0000ff;">this</span><span style="color: #000000;">.HashCode;
</span><span style="color: #008080;"> 41</span> <span style="color: #000000;">        }
</span><span style="color: #008080;"> 42</span> 
<span style="color: #008080;"> 43</span>         <span style="color: #0000ff;">private</span> Func&lt;HashCache, <span style="color: #0000ff;">string</span>&gt;<span style="color: #000000;"> GetUniqueString;
</span><span style="color: #008080;"> 44</span> 
<span style="color: #008080;"> 45</span>         <span style="color: #0000ff;">private</span> <span style="color: #0000ff;">bool</span> dirty = <span style="color: #0000ff;">true</span><span style="color: #000000;">;
</span><span style="color: #008080;"> 46</span> 
<span style="color: #008080;"> 47</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;summary&gt;</span>
<span style="color: #008080;"> 48</span>         <span style="color: #808080;">///</span><span style="color: #008000;"> 指明此cache需要更新才能用。
</span><span style="color: #008080;"> 49</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;/summary&gt;</span>
<span style="color: #008080;"> 50</span>         <span style="color: #0000ff;">public</span> <span style="color: #0000ff;">void</span> SetDirty() { <span style="color: #0000ff;">this</span>.dirty = <span style="color: #0000ff;">true</span><span style="color: #000000;">; }
</span><span style="color: #008080;"> 51</span> 
<span style="color: #008080;"> 52</span>         <span style="color: #0000ff;">private</span> <span style="color: #0000ff;">int</span><span style="color: #000000;"> hashCode;
</span><span style="color: #008080;"> 53</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;summary&gt;</span>
<span style="color: #008080;"> 54</span>         <span style="color: #808080;">///</span><span style="color: #008000;"> hash值。
</span><span style="color: #008080;"> 55</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;/summary&gt;</span>
<span style="color: #008080;"> 56</span>         <span style="color: #0000ff;">public</span> <span style="color: #0000ff;">int</span><span style="color: #000000;"> HashCode
</span><span style="color: #008080;"> 57</span> <span style="color: #000000;">        {
</span><span style="color: #008080;"> 58</span>             <span style="color: #0000ff;">get</span>
<span style="color: #008080;"> 59</span> <span style="color: #000000;">            {
</span><span style="color: #008080;"> 60</span>                 <span style="color: #0000ff;">if</span> (<span style="color: #0000ff;">this</span><span style="color: #000000;">.dirty)
</span><span style="color: #008080;"> 61</span> <span style="color: #000000;">                {
</span><span style="color: #008080;"> 62</span> <span style="color: #000000;">                    Update();
</span><span style="color: #008080;"> 63</span>  
<span style="color: #008080;"> 64</span>                     <span style="color: #0000ff;">this</span>.dirty = <span style="color: #0000ff;">false</span><span style="color: #000000;">;
</span><span style="color: #008080;"> 65</span> <span style="color: #000000;">                }
</span><span style="color: #008080;"> 66</span> 
<span style="color: #008080;"> 67</span>                 <span style="color: #0000ff;">return</span> <span style="color: #0000ff;">this</span><span style="color: #000000;">.hashCode;
</span><span style="color: #008080;"> 68</span> <span style="color: #000000;">            }
</span><span style="color: #008080;"> 69</span> <span style="color: #000000;">        }
</span><span style="color: #008080;"> 70</span> 
<span style="color: #008080;"> 71</span>         <span style="color: #0000ff;">private</span> <span style="color: #0000ff;">void</span><span style="color: #000000;"> Update()
</span><span style="color: #008080;"> 72</span> <span style="color: #000000;">        {
</span><span style="color: #008080;"> 73</span>             <span style="color: #0000ff;">string</span> str = GetUniqueString(<span style="color: #0000ff;">this</span><span style="color: #000000;">);
</span><span style="color: #008080;"> 74</span>             <span style="color: #0000ff;">int</span> hashCode =<span style="color: #000000;"> str.GetHashCode();
</span><span style="color: #008080;"> 75</span>             <span style="color: #0000ff;">this</span>.hashCode =<span style="color: #000000;"> hashCode;
</span><span style="color: #008080;"> 76</span> <span style="color: #0000ff;">#if</span> DEBUG
<span style="color: #008080;"> 77</span>             <span style="color: #0000ff;">this</span>.uniqueString = str;<span style="color: #008000;">//</span><span style="color: #008000;"> debug时可以看到可读的信息</span>
<span style="color: #008080;"> 78</span> <span style="color: #0000ff;">#else</span>
<span style="color: #008080;"> 79</span>             <span style="color: #0000ff;">this</span>.uniqueString = <span style="color: #0000ff;">string</span>.Format(<span style="color: #800000;">"</span><span style="color: #800000;">[{0}]</span><span style="color: #800000;">"</span>, hashCode);<span style="color: #008000;">//</span><span style="color: #008000;"> release后用最少的内存区分此对象</span>
<span style="color: #008080;"> 80</span> <span style="color: #0000ff;">#endif</span>
<span style="color: #008080;"> 81</span> <span style="color: #000000;">        }
</span><span style="color: #008080;"> 82</span> 
<span style="color: #008080;"> 83</span>         <span style="color: #008000;">//</span><span style="color: #008000;"> TODO: 功能稳定后应精简此字段的内容。</span>
<span style="color: #008080;"> 84</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;summary&gt;</span>
<span style="color: #008080;"> 85</span>         <span style="color: #808080;">///</span><span style="color: #008000;"> 功能稳定后应精简此字段的内容。
</span><span style="color: #008080;"> 86</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;/summary&gt;</span>
<span style="color: #008080;"> 87</span>         <span style="color: #0000ff;">private</span> <span style="color: #0000ff;">string</span> uniqueString = <span style="color: #0000ff;">string</span><span style="color: #000000;">.Empty;
</span><span style="color: #008080;"> 88</span> 
<span style="color: #008080;"> 89</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;summary&gt;</span>
<span style="color: #008080;"> 90</span>         <span style="color: #808080;">///</span><span style="color: #008000;"> 可唯一标识该对象的字符串。
</span><span style="color: #008080;"> 91</span>         <span style="color: #808080;">///</span><span style="color: #008000;"> 功能稳定后应精简此字段的内容。
</span><span style="color: #008080;"> 92</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;/summary&gt;</span>
<span style="color: #008080;"> 93</span>         <span style="color: #0000ff;">public</span> <span style="color: #0000ff;">string</span><span style="color: #000000;"> UniqueString
</span><span style="color: #008080;"> 94</span> <span style="color: #000000;">        {
</span><span style="color: #008080;"> 95</span>             <span style="color: #0000ff;">get</span>
<span style="color: #008080;"> 96</span> <span style="color: #000000;">            {
</span><span style="color: #008080;"> 97</span>                 <span style="color: #0000ff;">if</span> (<span style="color: #0000ff;">this</span><span style="color: #000000;">.dirty)
</span><span style="color: #008080;"> 98</span> <span style="color: #000000;">                {
</span><span style="color: #008080;"> 99</span> <span style="color: #000000;">                    Update();
</span><span style="color: #008080;">100</span> 
<span style="color: #008080;">101</span>                     <span style="color: #0000ff;">this</span>.dirty = <span style="color: #0000ff;">false</span><span style="color: #000000;">;
</span><span style="color: #008080;">102</span> <span style="color: #000000;">                }
</span><span style="color: #008080;">103</span> 
<span style="color: #008080;">104</span>                 <span style="color: #0000ff;">return</span> <span style="color: #0000ff;">this</span><span style="color: #000000;">.uniqueString;
</span><span style="color: #008080;">105</span> <span style="color: #000000;">            }
</span><span style="color: #008080;">106</span> <span style="color: #000000;">        }
</span><span style="color: #008080;">107</span> 
<span style="color: #008080;">108</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;summary&gt;</span>
<span style="color: #008080;">109</span>         <span style="color: #808080;">///</span><span style="color: #008000;"> 缓存一个对象的hash code。提高比较（==、!=、Equals、GetHashCode、Compare）的效率。
</span><span style="color: #008080;">110</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;/summary&gt;</span>
<span style="color: #008080;">111</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;param name="GetUniqueString"&gt;</span><span style="color: #008000;">获取一个可唯一标识此对象的字符串。</span><span style="color: #808080;">&lt;/param&gt;</span>
<span style="color: #008080;">112</span>         <span style="color: #0000ff;">public</span> HashCache(Func&lt;HashCache, <span style="color: #0000ff;">string</span>&gt;<span style="color: #000000;"> GetUniqueString)
</span><span style="color: #008080;">113</span> <span style="color: #000000;">        {
</span><span style="color: #008080;">114</span>             <span style="color: #0000ff;">if</span> (GetUniqueString == <span style="color: #0000ff;">null</span>) { <span style="color: #0000ff;">throw</span> <span style="color: #0000ff;">new</span><span style="color: #000000;"> ArgumentNullException(); }
</span><span style="color: #008080;">115</span> 
<span style="color: #008080;">116</span>             <span style="color: #0000ff;">this</span>.GetUniqueString =<span style="color: #000000;"> GetUniqueString;
</span><span style="color: #008080;">117</span> <span style="color: #000000;">        }
</span><span style="color: #008080;">118</span> 
<span style="color: #008080;">119</span>         <span style="color: #0000ff;">public</span> <span style="color: #0000ff;">override</span> <span style="color: #0000ff;">string</span><span style="color: #000000;"> ToString()
</span><span style="color: #008080;">120</span> <span style="color: #000000;">        {
</span><span style="color: #008080;">121</span>             <span style="color: #0000ff;">return</span> <span style="color: #0000ff;">this</span><span style="color: #000000;">.UniqueString;
</span><span style="color: #008080;">122</span> <span style="color: #000000;">        }
</span><span style="color: #008080;">123</span> 
<span style="color: #008080;">124</span>         <span style="color: #0000ff;">public</span> <span style="color: #0000ff;">int</span><span style="color: #000000;"> CompareTo(HashCache other)
</span><span style="color: #008080;">125</span> <span style="color: #000000;">        {
</span><span style="color: #008080;">126</span>             <span style="color: #0000ff;">if</span> (other == <span style="color: #0000ff;">null</span>) { <span style="color: #0000ff;">return</span> <span style="color: #800080;">1</span><span style="color: #000000;">; }
</span><span style="color: #008080;">127</span> 
<span style="color: #008080;">128</span>             <span style="color: #0000ff;">if</span> (<span style="color: #0000ff;">this</span>.HashCode &lt; other.HashCode)<span style="color: #008000;">//</span><span style="color: #008000;"> 如果用this.HashCode - other.HashCode &lt; 0，就会发生溢出，这个bug让我折腾了近8个小时。</span>
<span style="color: #008080;">129</span>             { <span style="color: #0000ff;">return</span> -<span style="color: #800080;">1</span><span style="color: #000000;">; }
</span><span style="color: #008080;">130</span>             <span style="color: #0000ff;">else</span> <span style="color: #0000ff;">if</span> (<span style="color: #0000ff;">this</span>.HashCode ==<span style="color: #000000;"> other.HashCode)
</span><span style="color: #008080;">131</span>             { <span style="color: #0000ff;">return</span> <span style="color: #800080;">0</span><span style="color: #000000;">; }
</span><span style="color: #008080;">132</span>             <span style="color: #0000ff;">else</span>
<span style="color: #008080;">133</span>             { <span style="color: #0000ff;">return</span> <span style="color: #800080;">1</span><span style="color: #000000;">; }
</span><span style="color: #008080;">134</span> <span style="color: #000000;">        }
</span><span style="color: #008080;">135</span>     }</pre>
</div>
<span class="cnblogs_code_collapse">HashCache</span></div>
<h2>有序集合</h2>
<p>如何判定两个集合（LR(0)State）相同？</p>
<p>一个LR(0)State是一个集合，集合内部的元素是没有先后顺序的区别的。但是为了比较两个State，其内部元素必须是有序的（这就可以用二分法进行插入和比较）。否则比较两个State会耗费太多时间。为了尽可能快地比较State，也要缓存State的HashCode。</p>
<p>有序集合的应用广泛，因此独立成类。</p>
<div class="cnblogs_code" onclick="cnblogs_code_show('bdb1f530-58b6-41aa-94a4-1a8c9b51a0a3')"><img id="code_img_closed_bdb1f530-58b6-41aa-94a4-1a8c9b51a0a3" class="code_img_closed" src="http://images.cnblogs.com/OutliningIndicators/ContractedBlock.gif" alt="" /><img id="code_img_opened_bdb1f530-58b6-41aa-94a4-1a8c9b51a0a3" class="code_img_opened" style="display: none;" onclick="cnblogs_code_hide('bdb1f530-58b6-41aa-94a4-1a8c9b51a0a3',event)" src="http://images.cnblogs.com/OutliningIndicators/ExpandedBlockStart.gif" alt="" />
<div id="cnblogs_code_open_bdb1f530-58b6-41aa-94a4-1a8c9b51a0a3" class="cnblogs_code_hide">
<pre><span style="color: #008080;"> 1</span>     <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;summary&gt;</span>
<span style="color: #008080;"> 2</span>     <span style="color: #808080;">///</span><span style="color: #008000;"> 经过优化的列表。插入新元素用二分法，速度更快，但使用者不能控制元素的位置。
</span><span style="color: #008080;"> 3</span>     <span style="color: #808080;">///</span><span style="color: #008000;"> 对于LALR(1)Compiler项目，只需支持&ldquo;添加元素&rdquo;的功能，所以我没有写修改和删除元素的功能。
</span><span style="color: #008080;"> 4</span>     <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;/summary&gt;</span>
<span style="color: #008080;"> 5</span>     <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;typeparam name="T"&gt;</span><span style="color: #008000;">元素也要支持快速比较。</span><span style="color: #808080;">&lt;/typeparam&gt;</span>
<span style="color: #008080;"> 6</span>     <span style="color: #0000ff;">public</span> <span style="color: #0000ff;">class</span> OrderedCollection&lt;T&gt;<span style="color: #000000;"> :
</span><span style="color: #008080;"> 7</span>         HashCache <span style="color: #008000;">//</span><span style="color: #008000;"> 快速比较两个OrderedCollection&lt;T&gt;是否相同。</span>
<span style="color: #008080;"> 8</span>         , IEnumerable&lt;T&gt; <span style="color: #008000;">//</span><span style="color: #008000;"> 可枚举该集合的元素。</span>
<span style="color: #008080;"> 9</span>         <span style="color: #0000ff;">where</span> T : HashCache <span style="color: #008000;">//</span><span style="color: #008000;"> 元素也要支持快速比较。</span>
<span style="color: #008080;">10</span> <span style="color: #000000;">    {
</span><span style="color: #008080;">11</span>         <span style="color: #0000ff;">private</span> List&lt;T&gt; list = <span style="color: #0000ff;">new</span> List&lt;T&gt;<span style="color: #000000;">();
</span><span style="color: #008080;">12</span>         <span style="color: #0000ff;">private</span> <span style="color: #0000ff;">string</span> seperator =<span style="color: #000000;"> Environment.NewLine;
</span><span style="color: #008080;">13</span> 
<span style="color: #008080;">14</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;summary&gt;</span>
<span style="color: #008080;">15</span>         <span style="color: #808080;">///</span><span style="color: #008000;"> 这是一个只能添加元素的集合，其元素是有序的，是按二分法插入的。
</span><span style="color: #008080;">16</span>         <span style="color: #808080;">///</span><span style="color: #008000;"> 但是使用者不能控制元素的顺序。
</span><span style="color: #008080;">17</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;/summary&gt;</span>
<span style="color: #008080;">18</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;param name="separator"&gt;</span><span style="color: #008000;">在Dump到流时用什么分隔符分隔各个元素？</span><span style="color: #808080;">&lt;/param&gt;</span>
<span style="color: #008080;">19</span>         <span style="color: #0000ff;">public</span> OrderedCollection(<span style="color: #0000ff;">string</span><span style="color: #000000;"> separator)
</span><span style="color: #008080;">20</span>             : <span style="color: #0000ff;">base</span><span style="color: #000000;">(GetUniqueString)
</span><span style="color: #008080;">21</span> <span style="color: #000000;">        {
</span><span style="color: #008080;">22</span>             <span style="color: #0000ff;">this</span>.seperator =<span style="color: #000000;"> separator;
</span><span style="color: #008080;">23</span> <span style="color: #000000;">        }
</span><span style="color: #008080;">24</span> 
<span style="color: #008080;">25</span>         <span style="color: #0000ff;">private</span> <span style="color: #0000ff;">static</span> <span style="color: #0000ff;">string</span><span style="color: #000000;"> GetUniqueString(HashCache cache)
</span><span style="color: #008080;">26</span> <span style="color: #000000;">        {
</span><span style="color: #008080;">27</span>             OrderedCollection&lt;T&gt; obj = cache <span style="color: #0000ff;">as</span> OrderedCollection&lt;T&gt;<span style="color: #000000;">;
</span><span style="color: #008080;">28</span>             <span style="color: #0000ff;">return</span><span style="color: #000000;"> obj.Dump();
</span><span style="color: #008080;">29</span> <span style="color: #000000;">        }
</span><span style="color: #008080;">30</span> 
<span style="color: #008080;">31</span>         <span style="color: #0000ff;">public</span> <span style="color: #0000ff;">virtual</span> <span style="color: #0000ff;">bool</span><span style="color: #000000;"> TryInsert(T item)
</span><span style="color: #008080;">32</span> <span style="color: #000000;">        {
</span><span style="color: #008080;">33</span>             <span style="color: #0000ff;">if</span> (<span style="color: #0000ff;">this</span><span style="color: #000000;">.list.TryBinaryInsert(item))
</span><span style="color: #008080;">34</span> <span style="color: #000000;">            {
</span><span style="color: #008080;">35</span>                 <span style="color: #0000ff;">this</span><span style="color: #000000;">.SetDirty();
</span><span style="color: #008080;">36</span>                 <span style="color: #0000ff;">return</span> <span style="color: #0000ff;">true</span><span style="color: #000000;">;
</span><span style="color: #008080;">37</span> <span style="color: #000000;">            }
</span><span style="color: #008080;">38</span>             <span style="color: #0000ff;">else</span>
<span style="color: #008080;">39</span> <span style="color: #000000;">            {
</span><span style="color: #008080;">40</span>                 <span style="color: #0000ff;">return</span> <span style="color: #0000ff;">false</span><span style="color: #000000;">;
</span><span style="color: #008080;">41</span> <span style="color: #000000;">            }
</span><span style="color: #008080;">42</span> <span style="color: #000000;">        }
</span><span style="color: #008080;">43</span> 
<span style="color: #008080;">44</span>         <span style="color: #0000ff;">public</span> <span style="color: #0000ff;">int</span><span style="color: #000000;"> IndexOf(T item)
</span><span style="color: #008080;">45</span> <span style="color: #000000;">        {
</span><span style="color: #008080;">46</span>             <span style="color: #0000ff;">return</span> <span style="color: #0000ff;">this</span><span style="color: #000000;">.list.BinarySearch(item);
</span><span style="color: #008080;">47</span> <span style="color: #000000;">        }
</span><span style="color: #008080;">48</span> 
<span style="color: #008080;">49</span>         <span style="color: #0000ff;">public</span> <span style="color: #0000ff;">bool</span><span style="color: #000000;"> Contains(T item)
</span><span style="color: #008080;">50</span> <span style="color: #000000;">        {
</span><span style="color: #008080;">51</span>             <span style="color: #0000ff;">int</span> index = <span style="color: #0000ff;">this</span><span style="color: #000000;">.list.BinarySearch(item);
</span><span style="color: #008080;">52</span>             <span style="color: #0000ff;">return</span> (<span style="color: #800080;">0</span> &lt;= index &amp;&amp; index &lt; <span style="color: #0000ff;">this</span><span style="color: #000000;">.list.Count);
</span><span style="color: #008080;">53</span> <span style="color: #000000;">        }
</span><span style="color: #008080;">54</span> 
<span style="color: #008080;">55</span>         <span style="color: #0000ff;">public</span> T <span style="color: #0000ff;">this</span>[<span style="color: #0000ff;">int</span> index] { <span style="color: #0000ff;">get</span> { <span style="color: #0000ff;">return</span> <span style="color: #0000ff;">this</span><span style="color: #000000;">.list[index]; } }
</span><span style="color: #008080;">56</span> 
<span style="color: #008080;">57</span>         <span style="color: #0000ff;">public</span> IEnumerator&lt;T&gt;<span style="color: #000000;"> GetEnumerator()
</span><span style="color: #008080;">58</span> <span style="color: #000000;">        {
</span><span style="color: #008080;">59</span>             <span style="color: #0000ff;">foreach</span> (<span style="color: #0000ff;">var</span> item <span style="color: #0000ff;">in</span> <span style="color: #0000ff;">this</span><span style="color: #000000;">.list)
</span><span style="color: #008080;">60</span> <span style="color: #000000;">            {
</span><span style="color: #008080;">61</span>                 <span style="color: #0000ff;">yield</span> <span style="color: #0000ff;">return</span><span style="color: #000000;"> item;
</span><span style="color: #008080;">62</span> <span style="color: #000000;">            }
</span><span style="color: #008080;">63</span> <span style="color: #000000;">        }
</span><span style="color: #008080;">64</span> 
<span style="color: #008080;">65</span>         <span style="color: #0000ff;">public</span> <span style="color: #0000ff;">int</span> Count { <span style="color: #0000ff;">get</span> { <span style="color: #0000ff;">return</span> <span style="color: #0000ff;">this</span><span style="color: #000000;">.list.Count; } }
</span><span style="color: #008080;">66</span> 
<span style="color: #008080;">67</span> <span style="color: #000000;">        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
</span><span style="color: #008080;">68</span> <span style="color: #000000;">        {
</span><span style="color: #008080;">69</span>             <span style="color: #0000ff;">return</span> <span style="color: #0000ff;">this</span><span style="color: #000000;">.GetEnumerator();
</span><span style="color: #008080;">70</span> <span style="color: #000000;">        }
</span><span style="color: #008080;">71</span> 
<span style="color: #008080;">72</span>         <span style="color: #0000ff;">public</span> <span style="color: #0000ff;">override</span> <span style="color: #0000ff;">void</span><span style="color: #000000;"> Dump(System.IO.TextWriter stream)
</span><span style="color: #008080;">73</span> <span style="color: #000000;">        {
</span><span style="color: #008080;">74</span>             <span style="color: #0000ff;">for</span> (<span style="color: #0000ff;">int</span> i = <span style="color: #800080;">0</span>; i &lt; <span style="color: #0000ff;">this</span>.list.Count; i++<span style="color: #000000;">)
</span><span style="color: #008080;">75</span> <span style="color: #000000;">            {
</span><span style="color: #008080;">76</span>                 <span style="color: #0000ff;">this</span><span style="color: #000000;">.list[i].Dump(stream);
</span><span style="color: #008080;">77</span>                 <span style="color: #0000ff;">if</span> (i + <span style="color: #800080;">1</span> &lt; <span style="color: #0000ff;">this</span><span style="color: #000000;">.list.Count)
</span><span style="color: #008080;">78</span> <span style="color: #000000;">                {
</span><span style="color: #008080;">79</span>                     stream.Write(<span style="color: #0000ff;">this</span><span style="color: #000000;">.seperator);
</span><span style="color: #008080;">80</span> <span style="color: #000000;">                }
</span><span style="color: #008080;">81</span> <span style="color: #000000;">            }
</span><span style="color: #008080;">82</span> <span style="color: #000000;">        }
</span><span style="color: #008080;">83</span>     }</pre>
</div>
<span class="cnblogs_code_collapse">OrderedCollection&lt;T&gt;</span></div>
<div>&nbsp;</div>
<p>其中有个TryBinaryInsert的扩展方法，用于向&nbsp;<span class="cnblogs_code">IList&lt;T&gt;</span>&nbsp;插入元素。这个方法我经过严格测试。如果有发现此方法的bug向我说明，我愿意奖励<span style="color: red;">￥100</span>元。</p>
<div class="cnblogs_code" onclick="cnblogs_code_show('e785f273-3423-4bf9-81bf-a904207d2a0d')"><img id="code_img_closed_e785f273-3423-4bf9-81bf-a904207d2a0d" class="code_img_closed" src="http://images.cnblogs.com/OutliningIndicators/ContractedBlock.gif" alt="" /><img id="code_img_opened_e785f273-3423-4bf9-81bf-a904207d2a0d" class="code_img_opened" style="display: none;" onclick="cnblogs_code_hide('e785f273-3423-4bf9-81bf-a904207d2a0d',event)" src="http://images.cnblogs.com/OutliningIndicators/ExpandedBlockStart.gif" alt="" />
<div id="cnblogs_code_open_e785f273-3423-4bf9-81bf-a904207d2a0d" class="cnblogs_code_hide">
<pre><span style="color: #008080;"> 1</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;summary&gt;</span>
<span style="color: #008080;"> 2</span>         <span style="color: #808080;">///</span><span style="color: #008000;"> 尝试插入新元素。如果存在相同的元素，就不插入，并返回false。否则返回true。
</span><span style="color: #008080;"> 3</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;/summary&gt;</span>
<span style="color: #008080;"> 4</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;typeparam name="T"&gt;&lt;/typeparam&gt;</span>
<span style="color: #008080;"> 5</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;param name="list"&gt;&lt;/param&gt;</span>
<span style="color: #008080;"> 6</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;param name="item"&gt;&lt;/param&gt;</span>
<span style="color: #008080;"> 7</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;returns&gt;&lt;/returns&gt;</span>
<span style="color: #008080;"> 8</span>         <span style="color: #0000ff;">public</span> <span style="color: #0000ff;">static</span> <span style="color: #0000ff;">bool</span> TryBinaryInsert&lt;T&gt;(<span style="color: #0000ff;">this</span> List&lt;T&gt;<span style="color: #000000;"> list, T item)
</span><span style="color: #008080;"> 9</span>             <span style="color: #0000ff;">where</span> T : IComparable&lt;T&gt;
<span style="color: #008080;">10</span> <span style="color: #000000;">        {
</span><span style="color: #008080;">11</span>             <span style="color: #0000ff;">bool</span> inserted = <span style="color: #0000ff;">false</span><span style="color: #000000;">;
</span><span style="color: #008080;">12</span> 
<span style="color: #008080;">13</span>             <span style="color: #0000ff;">if</span> (list == <span style="color: #0000ff;">null</span> || item == <span style="color: #0000ff;">null</span>) { <span style="color: #0000ff;">return</span><span style="color: #000000;"> inserted; }
</span><span style="color: #008080;">14</span> 
<span style="color: #008080;">15</span>             <span style="color: #0000ff;">int</span> left = <span style="color: #800080;">0</span>, right = list.Count - <span style="color: #800080;">1</span><span style="color: #000000;">;
</span><span style="color: #008080;">16</span>             <span style="color: #0000ff;">if</span> (right &lt; <span style="color: #800080;">0</span><span style="color: #000000;">)
</span><span style="color: #008080;">17</span> <span style="color: #000000;">            {
</span><span style="color: #008080;">18</span> <span style="color: #000000;">                list.Add(item);
</span><span style="color: #008080;">19</span>                 inserted = <span style="color: #0000ff;">true</span><span style="color: #000000;">;
</span><span style="color: #008080;">20</span> <span style="color: #000000;">            }
</span><span style="color: #008080;">21</span>             <span style="color: #0000ff;">else</span>
<span style="color: #008080;">22</span> <span style="color: #000000;">            {
</span><span style="color: #008080;">23</span>                 <span style="color: #0000ff;">while</span> (left &lt;<span style="color: #000000;"> right)
</span><span style="color: #008080;">24</span> <span style="color: #000000;">                {
</span><span style="color: #008080;">25</span>                     <span style="color: #0000ff;">int</span> mid = (left + right) / <span style="color: #800080;">2</span><span style="color: #000000;">;
</span><span style="color: #008080;">26</span>                     T current =<span style="color: #000000;"> list[mid];
</span><span style="color: #008080;">27</span>                     <span style="color: #0000ff;">int</span> result =<span style="color: #000000;"> item.CompareTo(current);
</span><span style="color: #008080;">28</span>                     <span style="color: #0000ff;">if</span> (result &lt; <span style="color: #800080;">0</span><span style="color: #000000;">)
</span><span style="color: #008080;">29</span>                     { right =<span style="color: #000000;"> mid; }
</span><span style="color: #008080;">30</span>                     <span style="color: #0000ff;">else</span> <span style="color: #0000ff;">if</span> (result == <span style="color: #800080;">0</span><span style="color: #000000;">)
</span><span style="color: #008080;">31</span>                     { left = mid; right =<span style="color: #000000;"> mid; }
</span><span style="color: #008080;">32</span>                     <span style="color: #0000ff;">else</span>
<span style="color: #008080;">33</span>                     { left = mid + <span style="color: #800080;">1</span><span style="color: #000000;">; }
</span><span style="color: #008080;">34</span> <span style="color: #000000;">                }
</span><span style="color: #008080;">35</span> <span style="color: #000000;">                {
</span><span style="color: #008080;">36</span>                     T current =<span style="color: #000000;"> list[left];
</span><span style="color: #008080;">37</span>                     <span style="color: #0000ff;">int</span> result =<span style="color: #000000;"> item.CompareTo(current);
</span><span style="color: #008080;">38</span>                     <span style="color: #0000ff;">if</span> (result &lt; <span style="color: #800080;">0</span><span style="color: #000000;">)
</span><span style="color: #008080;">39</span> <span style="color: #000000;">                    {
</span><span style="color: #008080;">40</span> <span style="color: #000000;">                        list.Insert(left, item);
</span><span style="color: #008080;">41</span>                         inserted = <span style="color: #0000ff;">true</span><span style="color: #000000;">;
</span><span style="color: #008080;">42</span> <span style="color: #000000;">                    }
</span><span style="color: #008080;">43</span>                     <span style="color: #0000ff;">else</span> <span style="color: #0000ff;">if</span> (result &gt; <span style="color: #800080;">0</span><span style="color: #000000;">)
</span><span style="color: #008080;">44</span> <span style="color: #000000;">                    {
</span><span style="color: #008080;">45</span>                         list.Insert(left + <span style="color: #800080;">1</span><span style="color: #000000;">, item);
</span><span style="color: #008080;">46</span>                         inserted = <span style="color: #0000ff;">true</span><span style="color: #000000;">;
</span><span style="color: #008080;">47</span> <span style="color: #000000;">                    }
</span><span style="color: #008080;">48</span> <span style="color: #000000;">                }
</span><span style="color: #008080;">49</span> <span style="color: #000000;">            }
</span><span style="color: #008080;">50</span> 
<span style="color: #008080;">51</span>             <span style="color: #0000ff;">return</span><span style="color: #000000;"> inserted;
</span><span style="color: #008080;">52</span>         }</pre>
</div>
<span class="cnblogs_code_collapse">TryBinaryInsert&lt;T&gt;(this IList&lt;T&gt; list, T item) where T : IComparable&lt;T&gt;</span></div>
<h2>迭代到不动点</h2>
<p>虎书中的算法大量使用了迭代到不动点的方式。</p>
<p><img src="http://images2015.cnblogs.com/blog/383191/201604/383191-20160415230937801-1274257256.png" alt="" /></p>
<p>这个方法虽好，却仍有可优化的余地。而且这属于核心的计算过程，也应当优化。</p>
<p>优化方法也简单，用一个Queue代替"迭代不动点"的方式即可。这就避免了很多不必要的重复计算。</p>
<div class="cnblogs_code" onclick="cnblogs_code_show('1976057a-2fc6-4407-9a36-412f38f7e588')"><img id="code_img_closed_1976057a-2fc6-4407-9a36-412f38f7e588" class="code_img_closed" src="http://images.cnblogs.com/OutliningIndicators/ContractedBlock.gif" alt="" /><img id="code_img_opened_1976057a-2fc6-4407-9a36-412f38f7e588" class="code_img_opened" style="display: none;" onclick="cnblogs_code_hide('1976057a-2fc6-4407-9a36-412f38f7e588',event)" src="http://images.cnblogs.com/OutliningIndicators/ExpandedBlockStart.gif" alt="" />
<div id="cnblogs_code_open_1976057a-2fc6-4407-9a36-412f38f7e588" class="cnblogs_code_hide">
<pre><span style="color: #008080;"> 1</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;summary&gt;</span>
<span style="color: #008080;"> 2</span>         <span style="color: #808080;">///</span><span style="color: #008000;"> LR(0)的Closure操作。
</span><span style="color: #008080;"> 3</span>         <span style="color: #808080;">///</span><span style="color: #008000;"> 补全一个状态。
</span><span style="color: #008080;"> 4</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;/summary&gt;</span>
<span style="color: #008080;"> 5</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;param name="list"&gt;&lt;/param&gt;</span>
<span style="color: #008080;"> 6</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;param name="state"&gt;&lt;/param&gt;</span>
<span style="color: #008080;"> 7</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;returns&gt;&lt;/returns&gt;</span>
<span style="color: #008080;"> 8</span>         <span style="color: #0000ff;">static</span> LR0State Closure(<span style="color: #0000ff;">this</span><span style="color: #000000;"> RegulationList list, LR0State state)
</span><span style="color: #008080;"> 9</span> <span style="color: #000000;">        {
</span><span style="color: #008080;">10</span>             Queue&lt;LR0Item&gt; queue = <span style="color: #0000ff;">new</span> Queue&lt;LR0Item&gt;<span style="color: #000000;">();
</span><span style="color: #008080;">11</span>             <span style="color: #0000ff;">foreach</span> (<span style="color: #0000ff;">var</span> item <span style="color: #0000ff;">in</span><span style="color: #000000;"> state)
</span><span style="color: #008080;">12</span> <span style="color: #000000;">            {
</span><span style="color: #008080;">13</span> <span style="color: #000000;">                queue.Enqueue(item);
</span><span style="color: #008080;">14</span> <span style="color: #000000;">            }
</span><span style="color: #008080;">15</span>             <span style="color: #0000ff;">while</span> (queue.Count &gt; <span style="color: #800080;">0</span><span style="color: #000000;">)
</span><span style="color: #008080;">16</span> <span style="color: #000000;">            {
</span><span style="color: #008080;">17</span>                 LR0Item item =<span style="color: #000000;"> queue.Dequeue();
</span><span style="color: #008080;">18</span>                 TreeNodeType node =<span style="color: #000000;"> item.GetNodeNext2Dot();
</span><span style="color: #008080;">19</span>                 <span style="color: #0000ff;">if</span> (node == <span style="color: #0000ff;">null</span>) { <span style="color: #0000ff;">continue</span><span style="color: #000000;">; }
</span><span style="color: #008080;">20</span> 
<span style="color: #008080;">21</span>                 <span style="color: #0000ff;">foreach</span> (<span style="color: #0000ff;">var</span> regulation <span style="color: #0000ff;">in</span><span style="color: #000000;"> list)
</span><span style="color: #008080;">22</span> <span style="color: #000000;">                {
</span><span style="color: #008080;">23</span>                     <span style="color: #0000ff;">if</span> (regulation.Left ==<span style="color: #000000;"> node)
</span><span style="color: #008080;">24</span> <span style="color: #000000;">                    {
</span><span style="color: #008080;">25</span>                         <span style="color: #0000ff;">var</span> newItem = <span style="color: #0000ff;">new</span> LR0Item(regulation, <span style="color: #800080;">0</span><span style="color: #000000;">);
</span><span style="color: #008080;">26</span>                         <span style="color: #0000ff;">if</span><span style="color: #000000;"> (state.TryInsert(newItem))
</span><span style="color: #008080;">27</span> <span style="color: #000000;">                        {
</span><span style="color: #008080;">28</span> <span style="color: #000000;">                            queue.Enqueue(newItem);
</span><span style="color: #008080;">29</span> <span style="color: #000000;">                        }
</span><span style="color: #008080;">30</span> <span style="color: #000000;">                    }
</span><span style="color: #008080;">31</span> <span style="color: #000000;">                }
</span><span style="color: #008080;">32</span> <span style="color: #000000;">            }
</span><span style="color: #008080;">33</span> 
<span style="color: #008080;">34</span>             <span style="color: #0000ff;">return</span><span style="color: #000000;"> state;
</span><span style="color: #008080;">35</span>         }</pre>
</div>
<span class="cnblogs_code_collapse">Closure</span></div>
<h1>测试</h1>
<p>以前我总喜欢做个非常精致的GUI来测试。现在发现没那个必要，简单的Console就可以了。</p>
<p><img src="http://images2015.cnblogs.com/blog/383191/201604/383191-20160415230941332-1762047700.png" alt="" /></p>
<p>详细的测试结果导出到文件里，可以慢慢查看分析。</p>
<div class="cnblogs_code" onclick="cnblogs_code_show('53b17321-6d04-4b29-98a3-3c0908843c4b')"><img id="code_img_closed_53b17321-6d04-4b29-98a3-3c0908843c4b" class="code_img_closed" src="http://images.cnblogs.com/OutliningIndicators/ContractedBlock.gif" alt="" /><img id="code_img_opened_53b17321-6d04-4b29-98a3-3c0908843c4b" class="code_img_opened" style="display: none;" onclick="cnblogs_code_hide('53b17321-6d04-4b29-98a3-3c0908843c4b',event)" src="http://images.cnblogs.com/OutliningIndicators/ExpandedBlockStart.gif" alt="" />
<div id="cnblogs_code_open_53b17321-6d04-4b29-98a3-3c0908843c4b" class="cnblogs_code_hide">
<pre><span style="color: #008080;"> 1</span> =====&gt;<span style="color: #000000;"> Processing .\TestCases\3_8.Grammar\3_8.Grammar
</span><span style="color: #008080;"> 2</span>     Get grammar <span style="color: #0000ff;">from</span><span style="color: #000000;"> source code...
</span><span style="color: #008080;"> 3</span> <span style="color: #000000;">        Dump 3_8.TokenList.log
</span><span style="color: #008080;"> 4</span> <span style="color: #000000;">        Dump 3_8.Tree.log
</span><span style="color: #008080;"> 5</span> <span style="color: #000000;">        Dump 3_8.FormatedGrammar.log
</span><span style="color: #008080;"> 6</span> <span style="color: #000000;">    Dump 3_8.FIRST.log
</span><span style="color: #008080;"> 7</span> <span style="color: #000000;">    Dump 3_8.FOLLOW.log
</span><span style="color: #008080;"> 8</span>     LR(<span style="color: #800080;">0</span><span style="color: #000000;">) parsing...
</span><span style="color: #008080;"> 9</span> <span style="color: #000000;">        Dump 3_8.State.log
</span><span style="color: #008080;">10</span> <span style="color: #000000;">        Dump 3_8.Edge.log
</span><span style="color: #008080;">11</span>         Dump LR(<span style="color: #800080;">0</span>) Compiler<span style="color: #800000;">'</span><span style="color: #800000;">s source code...</span>
<span style="color: #008080;">12</span> <span style="color: #000000;">    SLR parsing...
</span><span style="color: #008080;">13</span> <span style="color: #000000;">        Dump 3_8.State.log
</span><span style="color: #008080;">14</span> <span style="color: #000000;">        Dump 3_8.Edge.log
</span><span style="color: #008080;">15</span>         Dump SLR Compiler<span style="color: #800000;">'</span><span style="color: #800000;">s source code...</span>
<span style="color: #008080;">16</span>     LALR(<span style="color: #800080;">1</span><span style="color: #000000;">) parsing...
</span><span style="color: #008080;">17</span> <span style="color: #000000;">        Dump 3_8.State.log
</span><span style="color: #008080;">18</span> <span style="color: #000000;">        Dump 3_8.Edge.log
</span><span style="color: #008080;">19</span>         Dump LALR(<span style="color: #800080;">1</span>) Compiler<span style="color: #800000;">'</span><span style="color: #800000;">s source code...</span>
<span style="color: #008080;">20</span>     LR(<span style="color: #800080;">1</span><span style="color: #000000;">) parsing...
</span><span style="color: #008080;">21</span> <span style="color: #000000;">        Dump 3_8.State.log
</span><span style="color: #008080;">22</span> <span style="color: #000000;">        Dump 3_8.Edge.log
</span><span style="color: #008080;">23</span>         Dump LR(<span style="color: #800080;">1</span>) Compiler<span style="color: #800000;">'</span><span style="color: #800000;">s source code...</span>
<span style="color: #008080;">24</span>     Compiling 3_8 of LR(<span style="color: #800080;">0</span><span style="color: #000000;">) version
</span><span style="color: #008080;">25</span>     Test Code 3_8 of LR(<span style="color: #800080;">0</span><span style="color: #000000;">) version
</span><span style="color: #008080;">26</span> <span style="color: #000000;">    Compiling 3_8 of SLR version
</span><span style="color: #008080;">27</span> <span style="color: #000000;">    Test Code 3_8 of SLR version
</span><span style="color: #008080;">28</span>     Compiling 3_8 of LALR(<span style="color: #800080;">1</span><span style="color: #000000;">) version
</span><span style="color: #008080;">29</span>     Test Code 3_8 of LALR(<span style="color: #800080;">1</span><span style="color: #000000;">) version
</span><span style="color: #008080;">30</span>     Compiling 3_8 of LR(<span style="color: #800080;">1</span><span style="color: #000000;">) version
</span><span style="color: #008080;">31</span>     Test Code 3_8 of LR(<span style="color: #800080;">1</span><span style="color: #000000;">) version
</span><span style="color: #008080;">32</span> =====&gt;<span style="color: #000000;"> Processing .\TestCases\Demo.Grammar\Demo.Grammar
</span><span style="color: #008080;">33</span>     Get grammar <span style="color: #0000ff;">from</span><span style="color: #000000;"> source code...
</span><span style="color: #008080;">34</span> <span style="color: #000000;">        Dump Demo.TokenList.log
</span><span style="color: #008080;">35</span> <span style="color: #000000;">        Dump Demo.Tree.log
</span><span style="color: #008080;">36</span> <span style="color: #000000;">        Dump Demo.FormatedGrammar.log
</span><span style="color: #008080;">37</span> <span style="color: #000000;">    Dump Demo.FIRST.log
</span><span style="color: #008080;">38</span> <span style="color: #000000;">    Dump Demo.FOLLOW.log
</span><span style="color: #008080;">39</span>     LR(<span style="color: #800080;">0</span><span style="color: #000000;">) parsing...
</span><span style="color: #008080;">40</span> <span style="color: #000000;">        Dump Demo.State.log
</span><span style="color: #008080;">41</span> <span style="color: #000000;">        Dump Demo.Edge.log
</span><span style="color: #008080;">42</span>         Dump LR(<span style="color: #800080;">0</span>) Compiler<span style="color: #800000;">'</span><span style="color: #800000;">s source code...</span>
<span style="color: #008080;">43</span>         【Exists <span style="color: #800080;">5</span> Conflicts <span style="color: #0000ff;">in</span><span style="color: #000000;"> Parsingmap】
</span><span style="color: #008080;">44</span> <span style="color: #000000;">    SLR parsing...
</span><span style="color: #008080;">45</span> <span style="color: #000000;">        Dump Demo.State.log
</span><span style="color: #008080;">46</span> <span style="color: #000000;">        Dump Demo.Edge.log
</span><span style="color: #008080;">47</span>         Dump SLR Compiler<span style="color: #800000;">'</span><span style="color: #800000;">s source code...</span>
<span style="color: #008080;">48</span>         【Exists <span style="color: #800080;">2</span> Conflicts <span style="color: #0000ff;">in</span><span style="color: #000000;"> Parsingmap】
</span><span style="color: #008080;">49</span>     LALR(<span style="color: #800080;">1</span><span style="color: #000000;">) parsing...
</span><span style="color: #008080;">50</span> <span style="color: #000000;">        Dump Demo.State.log
</span><span style="color: #008080;">51</span> <span style="color: #000000;">        Dump Demo.Edge.log
</span><span style="color: #008080;">52</span>         Dump LALR(<span style="color: #800080;">1</span>) Compiler<span style="color: #800000;">'</span><span style="color: #800000;">s source code...</span>
<span style="color: #008080;">53</span>         【Exists <span style="color: #800080;">2</span> Conflicts <span style="color: #0000ff;">in</span><span style="color: #000000;"> Parsingmap】
</span><span style="color: #008080;">54</span>     LR(<span style="color: #800080;">1</span><span style="color: #000000;">) parsing...
</span><span style="color: #008080;">55</span> <span style="color: #000000;">        Dump Demo.State.log
</span><span style="color: #008080;">56</span> <span style="color: #000000;">        Dump Demo.Edge.log
</span><span style="color: #008080;">57</span>         Dump LR(<span style="color: #800080;">1</span>) Compiler<span style="color: #800000;">'</span><span style="color: #800000;">s source code...</span>
<span style="color: #008080;">58</span>         【Exists <span style="color: #800080;">6</span> Conflicts <span style="color: #0000ff;">in</span><span style="color: #000000;"> Parsingmap】
</span><span style="color: #008080;">59</span>     Compiling Demo of LR(<span style="color: #800080;">0</span><span style="color: #000000;">) version
</span><span style="color: #008080;">60</span>     Test Code Demo of LR(<span style="color: #800080;">0</span><span style="color: #000000;">) version
</span><span style="color: #008080;">61</span>         No need to Test Code with conflicts <span style="color: #0000ff;">in</span><span style="color: #000000;"> SyntaxParser
</span><span style="color: #008080;">62</span> <span style="color: #000000;">    Compiling Demo of SLR version
</span><span style="color: #008080;">63</span> <span style="color: #000000;">    Test Code Demo of SLR version
</span><span style="color: #008080;">64</span>         No need to Test Code with conflicts <span style="color: #0000ff;">in</span><span style="color: #000000;"> SyntaxParser
</span><span style="color: #008080;">65</span>     Compiling Demo of LALR(<span style="color: #800080;">1</span><span style="color: #000000;">) version
</span><span style="color: #008080;">66</span>     Test Code Demo of LALR(<span style="color: #800080;">1</span><span style="color: #000000;">) version
</span><span style="color: #008080;">67</span>         No need to Test Code with conflicts <span style="color: #0000ff;">in</span><span style="color: #000000;"> SyntaxParser
</span><span style="color: #008080;">68</span>     Compiling Demo of LR(<span style="color: #800080;">1</span><span style="color: #000000;">) version
</span><span style="color: #008080;">69</span>     Test Code Demo of LR(<span style="color: #800080;">1</span><span style="color: #000000;">) version
</span><span style="color: #008080;">70</span>         No need to Test Code with conflicts <span style="color: #0000ff;">in</span><span style="color: #000000;"> SyntaxParser
</span><span style="color: #008080;">71</span> =====&gt;<span style="color: #000000;"> Processing .\TestCases\GLSL.Grammar\GLSL.Grammar
</span><span style="color: #008080;">72</span>     Get grammar <span style="color: #0000ff;">from</span><span style="color: #000000;"> source code...
</span><span style="color: #008080;">73</span> <span style="color: #000000;">        Dump GLSL.TokenList.log
</span><span style="color: #008080;">74</span> <span style="color: #000000;">        Dump GLSL.Tree.log
</span><span style="color: #008080;">75</span> <span style="color: #000000;">        Dump GLSL.FormatedGrammar.log
</span><span style="color: #008080;">76</span> <span style="color: #000000;">    Dump GLSL.FIRST.log
</span><span style="color: #008080;">77</span> <span style="color: #000000;">    Dump GLSL.FOLLOW.log
</span><span style="color: #008080;">78</span>     LR(<span style="color: #800080;">0</span>) parsing...</pre>
</div>
<span class="cnblogs_code_collapse">Test.log</span></div>
<h1>初战GLSL</h1>
<p>测试完成后，就可以磨刀霍霍向GLSL文法了。由于GLSL文法比那些测试用的文法规模大的多，最初的版本里，计算过程居然花了好几个小时。最终出现内存不足的Exception，不得不进行优化。</p>
<p>书中给的GLSL文法也是比较奇葩。或许是有什么特别的门道我没有看懂吧。总之要降低难度先。</p>
<p>思路是，把grammar拆分成几个部分，分别处理。</p>
<p>首先是Expression，这是其他部分的基础。Expression部分是符合SLR的，非常好。</p>
<p>然后是statement，statement里有个else悬空的问题，幸好虎书里专门对这个问题做了说明，说可以容忍这个冲突，直接选择Shift，忽略Reduction即可。也非常好。</p>
<p>然后是function_definition，出乎意料的是这部分也是符合SLR的。Nice。</p>
<p>最后是declaration，这里遇到了意想不到的大麻烦。GLSL文法里有个&lt;TYPE_NAME&gt;。这个东西我研究了好久，最后发现他代表的含义竟然是"在读取源代码时动态发现的用户定义的类型"。比如&nbsp;<span class="cnblogs_code"><span style="color: #0000ff;">struct</span> LightInfo{ &hellip; }</span>&nbsp;，他代表的是&nbsp;<span class="cnblogs_code">LightInfo</span>&nbsp;这种类型。如果简单的用identifier代替&lt;TYPE_NAME&gt;，文法就会产生无法解决的冲突。</p>
<p>我只好就此打住，先去实现另一种更强的分析方式&mdash;&mdash;同步分析。</p>
<h1>同步分析</h1>
<p>现在，我的词法分析、语法分析是分开进行的。词法分析全部完成后，才把单词流交给语法分析器进行分析。为了<span style="color: red;">及时识别出用户自定义的类型</span>，这种方式完全不行，必须用"分析一个单词-&gt;语法分析-&gt;可能的语义分析-&gt;分析一个单词"这样的同步分析方式。例如下面的代码：</p>
<div class="cnblogs_code">
<pre><span style="color: #008080;">1</span> <span style="color: #0000ff;">struct</span><span style="color: #000000;"> LightInfo {  
</span><span style="color: #008080;">2</span>     vec4 Position; <span style="color: #008000;">//</span><span style="color: #008000;"> Light position in eyecoords.  </span>
<span style="color: #008080;">3</span>     vec3 La; <span style="color: #008000;">//</span><span style="color: #008000;"> Ambient light intensity  </span>
<span style="color: #008080;">4</span>     vec3 Ld; <span style="color: #008000;">//</span><span style="color: #008000;"> Diffuse light intensity  </span>
<span style="color: #008080;">5</span>     vec3 Ls; <span style="color: #008000;">//</span><span style="color: #008000;"> Specular light intensity  </span>
<span style="color: #008080;">6</span> <span style="color: #000000;">};  
</span><span style="color: #008080;">7</span> uniform LightInfo Light;</pre>
</div>
<div>&nbsp;</div>
<p>在读到第二个单词"LightInfo"后，就必须立即将这个"LightInfo"类型加到<span style="color: red;">用户自定义的类型表</span>里。这样，在继续读到"uniform LightInfo Light"里的"LightInfo"时，词法分析器才会知道"LightInfo"是一个<span style="color: red;">userDefinedType</span>，而不是一个随随便便的<span style="color: red;">identifier</span>。（对照上文的<span style="color: red;">文法的文法</span>，可见为实现一个看似不起眼的userDefinedType需要做多少事）</p>
<h2>前端分析器(FrontEndParser)</h2>
<p>既然要同步解析了，那么词法分析和语法分析就是结结实实绑在一起的过程，所有用个FrontEndParser封装一下就很有必要。其中的UserDefinedTypeCollection就用来记录用户自定义的类型。</p>
<div class="cnblogs_code">
<pre><span style="color: #008080;"> 1</span>     <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;summary&gt;</span>
<span style="color: #008080;"> 2</span>     <span style="color: #808080;">///</span><span style="color: #008000;"> 前端分析器。
</span><span style="color: #008080;"> 3</span>     <span style="color: #808080;">///</span><span style="color: #008000;"> 词法分析、语法分析、语义动作同步进行。
</span><span style="color: #008080;"> 4</span>     <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;/summary&gt;</span>
<span style="color: #008080;"> 5</span>     <span style="color: #0000ff;">public</span> <span style="color: #0000ff;">class</span><span style="color: #000000;"> FrontEndParser
</span><span style="color: #008080;"> 6</span> <span style="color: #000000;">    {
</span><span style="color: #008080;"> 7</span>         <span style="color: #0000ff;">private</span><span style="color: #000000;"> LexicalAnalyzer lexicalAnalyzer;
</span><span style="color: #008080;"> 8</span>         <span style="color: #0000ff;">private</span><span style="color: #000000;"> LRSyntaxParser syntaxParser;
</span><span style="color: #008080;"> 9</span> 
<span style="color: #008080;">10</span>         <span style="color: #0000ff;">public</span><span style="color: #000000;"> FrontEndParser(LexicalAnalyzer lexicalAnalyzer, LRSyntaxParser syntaxParser)
</span><span style="color: #008080;">11</span> <span style="color: #000000;">        {
</span><span style="color: #008080;">12</span>             <span style="color: #0000ff;">this</span>.lexicalAnalyzer =<span style="color: #000000;"> lexicalAnalyzer;
</span><span style="color: #008080;">13</span>             <span style="color: #0000ff;">this</span>.syntaxParser =<span style="color: #000000;"> syntaxParser;
</span><span style="color: #008080;">14</span> <span style="color: #000000;">        }
</span><span style="color: #008080;">15</span> 
<span style="color: #008080;">16</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;summary&gt;</span>
<span style="color: #008080;">17</span>         <span style="color: #808080;">///</span><span style="color: #008000;"> 词法分析、语法分析、语义动作同步进行。
</span><span style="color: #008080;">18</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;/summary&gt;</span>
<span style="color: #008080;">19</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;param name="sourceCode"&gt;&lt;/param&gt;</span>
<span style="color: #008080;">20</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;param name="tokenList"&gt;&lt;/param&gt;</span>
<span style="color: #008080;">21</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;returns&gt;&lt;/returns&gt;</span>
<span style="color: #008080;">22</span>         <span style="color: #0000ff;">public</span> SyntaxTree Parse(<span style="color: #0000ff;">string</span> sourceCode, <span style="color: #0000ff;">out</span><span style="color: #000000;"> TokenList tokenList)
</span><span style="color: #008080;">23</span> <span style="color: #000000;">        {
</span><span style="color: #008080;">24</span>             tokenList = <span style="color: #0000ff;">new</span><span style="color: #000000;"> TokenList();
</span><span style="color: #008080;">25</span>             UserDefinedTypeCollection userDefinedTypeTable = <span style="color: #0000ff;">new</span><span style="color: #000000;"> UserDefinedTypeCollection();
</span><span style="color: #008080;">26</span>             <span style="color: #0000ff;">this</span><span style="color: #000000;">.lexicalAnalyzer.StartAnalyzing(userDefinedTypeTable);
</span><span style="color: #008080;">27</span>             <span style="color: #0000ff;">this</span><span style="color: #000000;">.syntaxParser.StartParsing(userDefinedTypeTable);
</span><span style="color: #008080;">28</span>             <span style="color: #0000ff;">foreach</span> (<span style="color: #0000ff;">var</span> token <span style="color: #0000ff;">in</span> <span style="color: #0000ff;">this</span><span style="color: #000000;">.lexicalAnalyzer.AnalyzeStep(sourceCode))
</span><span style="color: #008080;">29</span> <span style="color: #000000;">            {
</span><span style="color: #008080;">30</span> <span style="color: #000000;">                tokenList.Add(token);
</span><span style="color: #008080;">31</span>                 <span style="color: #0000ff;">this</span><span style="color: #000000;">.syntaxParser.ParseStep(token);
</span><span style="color: #008080;">32</span> <span style="color: #000000;">            }
</span><span style="color: #008080;">33</span> 
<span style="color: #008080;">34</span>             SyntaxTree result = <span style="color: #0000ff;">this</span><span style="color: #000000;">.syntaxParser.StopParsing();
</span><span style="color: #008080;">35</span>             <span style="color: #0000ff;">return</span><span style="color: #000000;"> result;
</span><span style="color: #008080;">36</span> <span style="color: #000000;">        }
</span><span style="color: #008080;">37</span>     }</pre>
</div>
<h2>同步词法分析</h2>
<p>词法分析器需要每读取一个单词就返回之，等待语法分析、语义分析结束后再继续。C#的&nbsp;<span class="cnblogs_code"><span style="color: #0000ff;">yield</span> <span style="color: #0000ff;">return</span></span>&nbsp;语法糖真是甜。</p>
<div class="cnblogs_code" onclick="cnblogs_code_show('f47a6c2e-5918-45e5-9e49-a682e6990204')"><img id="code_img_closed_f47a6c2e-5918-45e5-9e49-a682e6990204" class="code_img_closed" src="http://images.cnblogs.com/OutliningIndicators/ContractedBlock.gif" alt="" /><img id="code_img_opened_f47a6c2e-5918-45e5-9e49-a682e6990204" class="code_img_opened" style="display: none;" onclick="cnblogs_code_hide('f47a6c2e-5918-45e5-9e49-a682e6990204',event)" src="http://images.cnblogs.com/OutliningIndicators/ExpandedBlockStart.gif" alt="" />
<div id="cnblogs_code_open_f47a6c2e-5918-45e5-9e49-a682e6990204" class="cnblogs_code_hide">
<pre><span style="color: #008080;"> 1</span>     <span style="color: #0000ff;">public</span> <span style="color: #0000ff;">abstract</span> <span style="color: #0000ff;">partial</span> <span style="color: #0000ff;">class</span><span style="color: #000000;"> LexicalAnalyzer
</span><span style="color: #008080;"> 2</span> <span style="color: #000000;">    {
</span><span style="color: #008080;"> 3</span>         <span style="color: #0000ff;">protected</span><span style="color: #000000;"> UserDefinedTypeCollection userDefinedTypeTable;
</span><span style="color: #008080;"> 4</span>         <span style="color: #0000ff;">private</span> <span style="color: #0000ff;">bool</span> inAnalyzingStep = <span style="color: #0000ff;">false</span><span style="color: #000000;">;
</span><span style="color: #008080;"> 5</span> 
<span style="color: #008080;"> 6</span>         <span style="color: #0000ff;">internal</span> <span style="color: #0000ff;">void</span><span style="color: #000000;"> StartAnalyzing(UserDefinedTypeCollection userDefinedTypeTable)
</span><span style="color: #008080;"> 7</span> <span style="color: #000000;">        {
</span><span style="color: #008080;"> 8</span>             <span style="color: #0000ff;">if</span> (!<span style="color: #000000;">inAnalyzingStep)
</span><span style="color: #008080;"> 9</span> <span style="color: #000000;">            {
</span><span style="color: #008080;">10</span>                 <span style="color: #0000ff;">this</span>.userDefinedTypeTable =<span style="color: #000000;"> userDefinedTypeTable;
</span><span style="color: #008080;">11</span>                 inAnalyzingStep = <span style="color: #0000ff;">true</span><span style="color: #000000;">;
</span><span style="color: #008080;">12</span> <span style="color: #000000;">            }
</span><span style="color: #008080;">13</span> <span style="color: #000000;">        }
</span><span style="color: #008080;">14</span> 
<span style="color: #008080;">15</span>         <span style="color: #0000ff;">internal</span> <span style="color: #0000ff;">void</span><span style="color: #000000;"> StopAnalyzing()
</span><span style="color: #008080;">16</span> <span style="color: #000000;">        {
</span><span style="color: #008080;">17</span>             <span style="color: #0000ff;">if</span><span style="color: #000000;"> (inAnalyzingStep)
</span><span style="color: #008080;">18</span> <span style="color: #000000;">            {
</span><span style="color: #008080;">19</span>                 <span style="color: #0000ff;">this</span>.userDefinedTypeTable = <span style="color: #0000ff;">null</span><span style="color: #000000;">;
</span><span style="color: #008080;">20</span>                 inAnalyzingStep = <span style="color: #0000ff;">false</span><span style="color: #000000;">;
</span><span style="color: #008080;">21</span> <span style="color: #000000;">            }
</span><span style="color: #008080;">22</span> <span style="color: #000000;">        }
</span><span style="color: #008080;">23</span> 
<span style="color: #008080;">24</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;summary&gt;</span>
<span style="color: #008080;">25</span>         <span style="color: #808080;">///</span><span style="color: #008000;"> 每次分析都返回一个</span><span style="color: #808080;">&lt;see cref="Token"/&gt;</span><span style="color: #008000;">。
</span><span style="color: #008080;">26</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;/summary&gt;</span>
<span style="color: #008080;">27</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;param name="sourceCode"&gt;&lt;/param&gt;</span>
<span style="color: #008080;">28</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;returns&gt;&lt;/returns&gt;</span>
<span style="color: #008080;">29</span>         <span style="color: #0000ff;">internal</span> IEnumerable&lt;Token&gt; AnalyzeStep(<span style="color: #0000ff;">string</span><span style="color: #000000;"> sourceCode)
</span><span style="color: #008080;">30</span> <span style="color: #000000;">        {
</span><span style="color: #008080;">31</span>             <span style="color: #0000ff;">if</span> (!inAnalyzingStep) { <span style="color: #0000ff;">throw</span> <span style="color: #0000ff;">new</span> Exception(<span style="color: #800000;">"</span><span style="color: #800000;">Must invoke this.StartAnalyzing() first!</span><span style="color: #800000;">"</span><span style="color: #000000;">); }
</span><span style="color: #008080;">32</span> 
<span style="color: #008080;">33</span>             <span style="color: #0000ff;">if</span> (!<span style="color: #0000ff;">string</span><span style="color: #000000;">.IsNullOrEmpty(sourceCode))
</span><span style="color: #008080;">34</span> <span style="color: #000000;">            {
</span><span style="color: #008080;">35</span>                 <span style="color: #0000ff;">var</span> context = <span style="color: #0000ff;">new</span><span style="color: #000000;"> AnalyzingContext(sourceCode);
</span><span style="color: #008080;">36</span>                 <span style="color: #0000ff;">int</span> count =<span style="color: #000000;"> sourceCode.Length;
</span><span style="color: #008080;">37</span> 
<span style="color: #008080;">38</span>                 <span style="color: #0000ff;">while</span> (context.NextLetterIndex &lt;<span style="color: #000000;"> count)
</span><span style="color: #008080;">39</span> <span style="color: #000000;">                {
</span><span style="color: #008080;">40</span>                     Token token =<span style="color: #000000;"> NextToken(context);
</span><span style="color: #008080;">41</span>                     <span style="color: #0000ff;">if</span> (token != <span style="color: #0000ff;">null</span><span style="color: #000000;">)
</span><span style="color: #008080;">42</span> <span style="color: #000000;">                    {
</span><span style="color: #008080;">43</span>                         <span style="color: #0000ff;">yield</span> <span style="color: #0000ff;">return</span><span style="color: #000000;"> token;
</span><span style="color: #008080;">44</span> <span style="color: #000000;">                    }
</span><span style="color: #008080;">45</span> <span style="color: #000000;">                }
</span><span style="color: #008080;">46</span> <span style="color: #000000;">            }
</span><span style="color: #008080;">47</span> 
<span style="color: #008080;">48</span>             <span style="color: #0000ff;">this</span><span style="color: #000000;">.StopAnalyzing();
</span><span style="color: #008080;">49</span> <span style="color: #000000;">        }
</span><span style="color: #008080;">50</span>     }</pre>
</div>
<span class="cnblogs_code_collapse">同步词法分析</span></div>
<h2>同步语法/语义分析</h2>
<p>每次只获取一个新单词，据此执行可能的分析步骤。如果分析动作还绑定了语义分析（这里是为了找到自定义类型），也执行之。</p>
<div class="cnblogs_code" onclick="cnblogs_code_show('4c4ed39c-9884-43c4-8514-c39c94dd0341')"><img id="code_img_closed_4c4ed39c-9884-43c4-8514-c39c94dd0341" class="code_img_closed" src="http://images.cnblogs.com/OutliningIndicators/ContractedBlock.gif" alt="" /><img id="code_img_opened_4c4ed39c-9884-43c4-8514-c39c94dd0341" class="code_img_opened" style="display: none;" onclick="cnblogs_code_hide('4c4ed39c-9884-43c4-8514-c39c94dd0341',event)" src="http://images.cnblogs.com/OutliningIndicators/ExpandedBlockStart.gif" alt="" />
<div id="cnblogs_code_open_4c4ed39c-9884-43c4-8514-c39c94dd0341" class="cnblogs_code_hide">
<pre><span style="color: #008080;"> 1</span>     <span style="color: #0000ff;">public</span> <span style="color: #0000ff;">abstract</span> <span style="color: #0000ff;">partial</span> <span style="color: #0000ff;">class</span><span style="color: #000000;"> LRSyntaxParser
</span><span style="color: #008080;"> 2</span> <span style="color: #000000;">    {
</span><span style="color: #008080;"> 3</span>         <span style="color: #0000ff;">bool</span> inParsingStep = <span style="color: #0000ff;">false</span><span style="color: #000000;">;
</span><span style="color: #008080;"> 4</span> <span style="color: #000000;">        ParsingStepContext parsingStepContext;
</span><span style="color: #008080;"> 5</span> 
<span style="color: #008080;"> 6</span>         <span style="color: #0000ff;">internal</span> <span style="color: #0000ff;">void</span><span style="color: #000000;"> StartParsing(UserDefinedTypeCollection userDefinedTypeTable)
</span><span style="color: #008080;"> 7</span> <span style="color: #000000;">        {
</span><span style="color: #008080;"> 8</span>             <span style="color: #0000ff;">if</span> (!<span style="color: #000000;">inParsingStep)
</span><span style="color: #008080;"> 9</span> <span style="color: #000000;">            {
</span><span style="color: #008080;">10</span>                 LRParsingMap parsingMap =<span style="color: #000000;"> GetParsingMap();
</span><span style="color: #008080;">11</span>                 RegulationList grammar =<span style="color: #000000;"> GetGrammar();
</span><span style="color: #008080;">12</span>                 <span style="color: #0000ff;">var</span> tokenTypeConvertor = <span style="color: #0000ff;">new</span><span style="color: #000000;"> TokenType2TreeNodeType();
</span><span style="color: #008080;">13</span>                 parsingStepContext = <span style="color: #0000ff;">new</span><span style="color: #000000;"> ParsingStepContext(
</span><span style="color: #008080;">14</span> <span style="color: #000000;">                    grammar, parsingMap, tokenTypeConvertor, userDefinedTypeTable);
</span><span style="color: #008080;">15</span>                 inParsingStep = <span style="color: #0000ff;">true</span><span style="color: #000000;">;
</span><span style="color: #008080;">16</span> <span style="color: #000000;">            }
</span><span style="color: #008080;">17</span> <span style="color: #000000;">        }
</span><span style="color: #008080;">18</span> 
<span style="color: #008080;">19</span>         <span style="color: #0000ff;">internal</span><span style="color: #000000;"> SyntaxTree StopParsing()
</span><span style="color: #008080;">20</span> <span style="color: #000000;">        {
</span><span style="color: #008080;">21</span>             SyntaxTree result = <span style="color: #0000ff;">null</span><span style="color: #000000;">;
</span><span style="color: #008080;">22</span>             <span style="color: #0000ff;">if</span><span style="color: #000000;"> (inParsingStep)
</span><span style="color: #008080;">23</span> <span style="color: #000000;">            {
</span><span style="color: #008080;">24</span>                 result =<span style="color: #000000;"> ParseStep(Token.endOfTokenList);
</span><span style="color: #008080;">25</span>                 parsingStepContext.TokenList.RemoveAt(parsingStepContext.TokenList.Count - <span style="color: #800080;">1</span><span style="color: #000000;">);
</span><span style="color: #008080;">26</span>                 parsingStepContext = <span style="color: #0000ff;">null</span><span style="color: #000000;">;
</span><span style="color: #008080;">27</span>                 inParsingStep = <span style="color: #0000ff;">false</span><span style="color: #000000;">;
</span><span style="color: #008080;">28</span> <span style="color: #000000;">            }
</span><span style="color: #008080;">29</span> 
<span style="color: #008080;">30</span>             <span style="color: #0000ff;">return</span><span style="color: #000000;"> result;
</span><span style="color: #008080;">31</span> <span style="color: #000000;">        }
</span><span style="color: #008080;">32</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;summary&gt;</span>
<span style="color: #008080;">33</span>         <span style="color: #808080;">///</span><span style="color: #008000;"> 获取归约动作对应的语义动作。
</span><span style="color: #008080;">34</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;/summary&gt;</span>
<span style="color: #008080;">35</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;param name="parsingAction"&gt;&lt;/param&gt;</span>
<span style="color: #008080;">36</span>         <span style="color: #808080;">///</span> <span style="color: #808080;">&lt;returns&gt;&lt;/returns&gt;</span>
<span style="color: #008080;">37</span>         <span style="color: #0000ff;">protected</span> <span style="color: #0000ff;">virtual</span> Action&lt;ParsingStepContext&gt;<span style="color: #000000;"> GetSemanticAction(LRParsingAction parsingAction)
</span><span style="color: #008080;">38</span> <span style="color: #000000;">        {
</span><span style="color: #008080;">39</span>             <span style="color: #0000ff;">return</span> <span style="color: #0000ff;">null</span><span style="color: #000000;">;
</span><span style="color: #008080;">40</span> <span style="color: #000000;">        }
</span><span style="color: #008080;">41</span> 
<span style="color: #008080;">42</span>         <span style="color: #0000ff;">internal</span><span style="color: #000000;"> SyntaxTree ParseStep(Token token)
</span><span style="color: #008080;">43</span> <span style="color: #000000;">        {
</span><span style="color: #008080;">44</span>             <span style="color: #0000ff;">if</span> (!inParsingStep) { <span style="color: #0000ff;">throw</span> <span style="color: #0000ff;">new</span> Exception(<span style="color: #800000;">"</span><span style="color: #800000;">Must invoke this.StartParsing() first!</span><span style="color: #800000;">"</span><span style="color: #000000;">); }
</span><span style="color: #008080;">45</span> 
<span style="color: #008080;">46</span> <span style="color: #000000;">            parsingStepContext.AddToken(token);
</span><span style="color: #008080;">47</span> 
<span style="color: #008080;">48</span>             <span style="color: #0000ff;">while</span> (parsingStepContext.CurrentTokenIndex &lt;<span style="color: #000000;"> parsingStepContext.TokenList.Count)
</span><span style="color: #008080;">49</span> <span style="color: #000000;">            {
</span><span style="color: #008080;">50</span>                 <span style="color: #008000;">//</span><span style="color: #008000;"> 语法分析</span>
<span style="color: #008080;">51</span>                 TreeNodeType nodeType =<span style="color: #000000;"> parsingStepContext.CurrentNodeType();
</span><span style="color: #008080;">52</span>                 <span style="color: #0000ff;">int</span> stateId =<span style="color: #000000;"> parsingStepContext.StateIdStack.Peek();
</span><span style="color: #008080;">53</span>                 LRParsingAction action =<span style="color: #000000;"> parsingStepContext.ParsingMap.GetAction(stateId, nodeType);
</span><span style="color: #008080;">54</span>                 <span style="color: #0000ff;">int</span> currentTokenIndex =<span style="color: #000000;"> action.Execute(parsingStepContext);
</span><span style="color: #008080;">55</span>                 parsingStepContext.CurrentTokenIndex =<span style="color: #000000;"> currentTokenIndex;
</span><span style="color: #008080;">56</span>                 <span style="color: #008000;">//</span><span style="color: #008000;"> 语义分析</span>
<span style="color: #008080;">57</span>                 Action&lt;ParsingStepContext&gt; semanticAction =<span style="color: #000000;"> GetSemanticAction(action);
</span><span style="color: #008080;">58</span>                 <span style="color: #0000ff;">if</span> (semanticAction != <span style="color: #0000ff;">null</span><span style="color: #000000;">)
</span><span style="color: #008080;">59</span> <span style="color: #000000;">                {
</span><span style="color: #008080;">60</span> <span style="color: #000000;">                    semanticAction(parsingStepContext);
</span><span style="color: #008080;">61</span> <span style="color: #000000;">                }
</span><span style="color: #008080;">62</span> <span style="color: #000000;">            }
</span><span style="color: #008080;">63</span> 
<span style="color: #008080;">64</span>             <span style="color: #0000ff;">if</span> (parsingStepContext.TreeStack.Count &gt; <span style="color: #800080;">0</span><span style="color: #000000;">)
</span><span style="color: #008080;">65</span> <span style="color: #000000;">            {
</span><span style="color: #008080;">66</span>                 <span style="color: #0000ff;">return</span><span style="color: #000000;"> parsingStepContext.TreeStack.Peek();
</span><span style="color: #008080;">67</span> <span style="color: #000000;">            }
</span><span style="color: #008080;">68</span>             <span style="color: #0000ff;">else</span>
<span style="color: #008080;">69</span> <span style="color: #000000;">            {
</span><span style="color: #008080;">70</span>                 <span style="color: #0000ff;">return</span> <span style="color: #0000ff;">new</span><span style="color: #000000;"> SyntaxTree();
</span><span style="color: #008080;">71</span> <span style="color: #000000;">            }
</span><span style="color: #008080;">72</span> <span style="color: #000000;">        }
</span><span style="color: #008080;">73</span>     }</pre>
</div>
<span class="cnblogs_code_collapse">同步语法/语义分析</span></div>
<div>&nbsp;</div>
<h1>再战GLSL</h1>
<p>此时武器终于齐备。</p>
<h2>文法-&gt;解析器</h2>
<p>为下面的GLSL文法生成解析器，我的笔记本花费大概10分钟左右。</p>
<div class="cnblogs_code" onclick="cnblogs_code_show('45564f23-8a78-42aa-ba72-6b1b6a7c946b')"><img id="code_img_closed_45564f23-8a78-42aa-ba72-6b1b6a7c946b" class="code_img_closed" src="http://images.cnblogs.com/OutliningIndicators/ContractedBlock.gif" alt="" /><img id="code_img_opened_45564f23-8a78-42aa-ba72-6b1b6a7c946b" class="code_img_opened" style="display: none;" onclick="cnblogs_code_hide('45564f23-8a78-42aa-ba72-6b1b6a7c946b',event)" src="http://images.cnblogs.com/OutliningIndicators/ExpandedBlockStart.gif" alt="" />
<div id="cnblogs_code_open_45564f23-8a78-42aa-ba72-6b1b6a7c946b" class="cnblogs_code_hide">
<pre><span style="color: #008080;">  1</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">translation_unit</span><span style="color: #0000ff;">&gt;</span> ::= <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">external_declaration</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ;
</span><span style="color: #008080;">  2</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">translation_unit</span><span style="color: #0000ff;">&gt;</span> ::= <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">translation_unit</span><span style="color: #0000ff;">&gt;</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">external_declaration</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ;
</span><span style="color: #008080;">  3</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">external_declaration</span><span style="color: #0000ff;">&gt;</span> ::= <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">function_definition</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ;
</span><span style="color: #008080;">  4</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">external_declaration</span><span style="color: #0000ff;">&gt;</span> ::= <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">declaration</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ;
</span><span style="color: #008080;">  5</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">function_definition</span><span style="color: #0000ff;">&gt;</span> ::= <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">function_prototype</span><span style="color: #0000ff;">&gt;</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">compound_statement_no_new_scope</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ;
</span><span style="color: #008080;">  6</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">variable_identifier</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= identifier ;
</span><span style="color: #008080;">  7</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">primary_expression</span><span style="color: #0000ff;">&gt;</span> ::= <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">variable_identifier</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ;
</span><span style="color: #008080;">  8</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">primary_expression</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= number ;
</span><span style="color: #008080;">  9</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">primary_expression</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= number ;
</span><span style="color: #008080;"> 10</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">primary_expression</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= number ;
</span><span style="color: #008080;"> 11</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">primary_expression</span><span style="color: #0000ff;">&gt;</span> ::= <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">BOOLCONSTANT</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ;
</span><span style="color: #008080;"> 12</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">primary_expression</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= number ;
</span><span style="color: #008080;"> 13</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">primary_expression</span><span style="color: #0000ff;">&gt;</span> ::= "(" <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">expression</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ")" ;
</span><span style="color: #008080;"> 14</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">BOOLCONSTANT</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "true" ;
</span><span style="color: #008080;"> 15</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">BOOLCONSTANT</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "false" ;
</span><span style="color: #008080;"> 16</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">postfix_expression</span><span style="color: #0000ff;">&gt;</span> ::= <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">primary_expression</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ;
</span><span style="color: #008080;"> 17</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">postfix_expression</span><span style="color: #0000ff;">&gt;</span> ::= <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">postfix_expression</span><span style="color: #0000ff;">&gt;</span> "[" <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">integer_expression</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> "]" ;
</span><span style="color: #008080;"> 18</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">postfix_expression</span><span style="color: #0000ff;">&gt;</span> ::= <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">function_call</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ;
</span><span style="color: #008080;"> 19</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">postfix_expression</span><span style="color: #0000ff;">&gt;</span> ::= <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">postfix_expression</span><span style="color: #0000ff;">&gt;</span> "." <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">FIELD_SELECTION</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ;
</span><span style="color: #008080;"> 20</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">postfix_expression</span><span style="color: #0000ff;">&gt;</span> ::= <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">postfix_expression</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> "++" ;
</span><span style="color: #008080;"> 21</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">postfix_expression</span><span style="color: #0000ff;">&gt;</span> ::= <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">postfix_expression</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> "--" ;
</span><span style="color: #008080;"> 22</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">FIELD_SELECTION</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= identifier ;
</span><span style="color: #008080;"> 23</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">integer_expression</span><span style="color: #0000ff;">&gt;</span> ::= <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">expression</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ;
</span><span style="color: #008080;"> 24</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">function_call</span><span style="color: #0000ff;">&gt;</span> ::= <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">function_call_or_method</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ;
</span><span style="color: #008080;"> 25</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">function_call_or_method</span><span style="color: #0000ff;">&gt;</span> ::= <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">function_call_generic</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ;
</span><span style="color: #008080;"> 26</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">function_call_generic</span><span style="color: #0000ff;">&gt;</span> ::= <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">function_call_header_with_parameters</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ")" ;
</span><span style="color: #008080;"> 27</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">function_call_generic</span><span style="color: #0000ff;">&gt;</span> ::= <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">function_call_header_no_parameters</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ")" ;
</span><span style="color: #008080;"> 28</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">function_call_header_no_parameters</span><span style="color: #0000ff;">&gt;</span> ::= <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">function_call_header</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> "void" ;
</span><span style="color: #008080;"> 29</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">function_call_header_no_parameters</span><span style="color: #0000ff;">&gt;</span> ::= <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">function_call_header</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ;
</span><span style="color: #008080;"> 30</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">function_call_header_with_parameters</span><span style="color: #0000ff;">&gt;</span> ::= <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">function_call_header</span><span style="color: #0000ff;">&gt;</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">assignment_expression</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ;
</span><span style="color: #008080;"> 31</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">function_call_header_with_parameters</span><span style="color: #0000ff;">&gt;</span> ::= <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">function_call_header_with_parameters</span><span style="color: #0000ff;">&gt;</span> "," <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">assignment_expression</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ;
</span><span style="color: #008080;"> 32</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">function_call_header</span><span style="color: #0000ff;">&gt;</span> ::= <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">function_identifier</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> "(" ;
</span><span style="color: #008080;"> 33</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">function_identifier</span><span style="color: #0000ff;">&gt;</span> ::= <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">type_specifier</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ;
</span><span style="color: #008080;"> 34</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">function_identifier</span><span style="color: #0000ff;">&gt;</span> ::= <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">postfix_expression</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ;
</span><span style="color: #008080;"> 35</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">unary_expression</span><span style="color: #0000ff;">&gt;</span> ::= <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">postfix_expression</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ;
</span><span style="color: #008080;"> 36</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">unary_expression</span><span style="color: #0000ff;">&gt;</span> ::= "++" <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">unary_expression</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ;
</span><span style="color: #008080;"> 37</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">unary_expression</span><span style="color: #0000ff;">&gt;</span> ::= "--" <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">unary_expression</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ;
</span><span style="color: #008080;"> 38</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">unary_expression</span><span style="color: #0000ff;">&gt;</span> ::= <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">unary_operator</span><span style="color: #0000ff;">&gt;</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">unary_expression</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ;
</span><span style="color: #008080;"> 39</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">unary_operator</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "+" ;
</span><span style="color: #008080;"> 40</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">unary_operator</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "-" ;
</span><span style="color: #008080;"> 41</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">unary_operator</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "!" ;
</span><span style="color: #008080;"> 42</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">unary_operator</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "~" ;
</span><span style="color: #008080;"> 43</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">multiplicative_expression</span><span style="color: #0000ff;">&gt;</span> ::= <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">unary_expression</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ;
</span><span style="color: #008080;"> 44</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">multiplicative_expression</span><span style="color: #0000ff;">&gt;</span> ::= <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">multiplicative_expression</span><span style="color: #0000ff;">&gt;</span> "*" <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">unary_expression</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ;
</span><span style="color: #008080;"> 45</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">multiplicative_expression</span><span style="color: #0000ff;">&gt;</span> ::= <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">multiplicative_expression</span><span style="color: #0000ff;">&gt;</span> "/" <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">unary_expression</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ;
</span><span style="color: #008080;"> 46</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">multiplicative_expression</span><span style="color: #0000ff;">&gt;</span> ::= <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">multiplicative_expression</span><span style="color: #0000ff;">&gt;</span> "%" <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">unary_expression</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ;
</span><span style="color: #008080;"> 47</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">additive_expression</span><span style="color: #0000ff;">&gt;</span> ::= <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">multiplicative_expression</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ;
</span><span style="color: #008080;"> 48</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">additive_expression</span><span style="color: #0000ff;">&gt;</span> ::= <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">additive_expression</span><span style="color: #0000ff;">&gt;</span> "+" <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">multiplicative_expression</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ;
</span><span style="color: #008080;"> 49</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">additive_expression</span><span style="color: #0000ff;">&gt;</span> ::= <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">additive_expression</span><span style="color: #0000ff;">&gt;</span> "-" <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">multiplicative_expression</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ;
</span><span style="color: #008080;"> 50</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">shift_expression</span><span style="color: #0000ff;">&gt;</span> ::= <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">additive_expression</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ;
</span><span style="color: #008080;"> 51</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">shift_expression</span><span style="color: #0000ff;">&gt;</span> ::= <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">shift_expression</span><span style="color: #0000ff;">&gt;</span> "<span style="color: #0000ff;">&lt;</span><span style="color: #800000;">&lt;" &lt;additive_expression</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ;
</span><span style="color: #008080;"> 52</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">shift_expression</span><span style="color: #0000ff;">&gt;</span> ::= <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">shift_expression</span><span style="color: #0000ff;">&gt;</span> "&gt;&gt;" <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">additive_expression</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ;
</span><span style="color: #008080;"> 53</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">relational_expression</span><span style="color: #0000ff;">&gt;</span> ::= <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">shift_expression</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ;
</span><span style="color: #008080;"> 54</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">relational_expression</span><span style="color: #0000ff;">&gt;</span> ::= <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">relational_expression</span><span style="color: #0000ff;">&gt;</span> "<span style="color: #0000ff;">&lt;</span><span style="color: #800000;">" &lt;shift_expression</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ;
</span><span style="color: #008080;"> 55</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">relational_expression</span><span style="color: #0000ff;">&gt;</span> ::= <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">relational_expression</span><span style="color: #0000ff;">&gt;</span> "&gt;" <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">shift_expression</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ;
</span><span style="color: #008080;"> 56</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">relational_expression</span><span style="color: #0000ff;">&gt;</span> ::= <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">relational_expression</span><span style="color: #0000ff;">&gt;</span> "<span style="color: #0000ff;">&lt;</span><span style="color: #800000;">=" &lt;shift_expression</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ;
</span><span style="color: #008080;"> 57</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">relational_expression</span><span style="color: #0000ff;">&gt;</span> ::= <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">relational_expression</span><span style="color: #0000ff;">&gt;</span> "&gt;=" <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">shift_expression</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ;
</span><span style="color: #008080;"> 58</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">equality_expression</span><span style="color: #0000ff;">&gt;</span> ::= <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">relational_expression</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ;
</span><span style="color: #008080;"> 59</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">equality_expression</span><span style="color: #0000ff;">&gt;</span> ::= <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">equality_expression</span><span style="color: #0000ff;">&gt;</span> "==" <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">relational_expression</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ;
</span><span style="color: #008080;"> 60</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">equality_expression</span><span style="color: #0000ff;">&gt;</span> ::= <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">equality_expression</span><span style="color: #0000ff;">&gt;</span> "!=" <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">relational_expression</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ;
</span><span style="color: #008080;"> 61</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">and_expression</span><span style="color: #0000ff;">&gt;</span> ::= <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">equality_expression</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ;
</span><span style="color: #008080;"> 62</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">and_expression</span><span style="color: #0000ff;">&gt;</span> ::= <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">and_expression</span><span style="color: #0000ff;">&gt;</span> "&amp;" <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">equality_expression</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ;
</span><span style="color: #008080;"> 63</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">exclusive_or_expression</span><span style="color: #0000ff;">&gt;</span> ::= <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">and_expression</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ;
</span><span style="color: #008080;"> 64</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">exclusive_or_expression</span><span style="color: #0000ff;">&gt;</span> ::= <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">exclusive_or_expression</span><span style="color: #0000ff;">&gt;</span> "^" <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">and_expression</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ;
</span><span style="color: #008080;"> 65</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">inclusive_or_expression</span><span style="color: #0000ff;">&gt;</span> ::= <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">exclusive_or_expression</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ;
</span><span style="color: #008080;"> 66</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">inclusive_or_expression</span><span style="color: #0000ff;">&gt;</span> ::= <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">inclusive_or_expression</span><span style="color: #0000ff;">&gt;</span> "|" <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">exclusive_or_expression</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ;
</span><span style="color: #008080;"> 67</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">logical_and_expression</span><span style="color: #0000ff;">&gt;</span> ::= <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">inclusive_or_expression</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ;
</span><span style="color: #008080;"> 68</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">logical_and_expression</span><span style="color: #0000ff;">&gt;</span> ::= <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">logical_and_expression</span><span style="color: #0000ff;">&gt;</span> "&amp;&amp;" <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">inclusive_or_expression</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ;
</span><span style="color: #008080;"> 69</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">logical_xor_expression</span><span style="color: #0000ff;">&gt;</span> ::= <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">logical_and_expression</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ;
</span><span style="color: #008080;"> 70</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">logical_xor_expression</span><span style="color: #0000ff;">&gt;</span> ::= <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">logical_xor_expression</span><span style="color: #0000ff;">&gt;</span> "^^" <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">logical_and_expression</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ;
</span><span style="color: #008080;"> 71</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">logical_or_expression</span><span style="color: #0000ff;">&gt;</span> ::= <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">logical_xor_expression</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ;
</span><span style="color: #008080;"> 72</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">logical_or_expression</span><span style="color: #0000ff;">&gt;</span> ::= <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">logical_or_expression</span><span style="color: #0000ff;">&gt;</span> "||" <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">logical_xor_expression</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ;
</span><span style="color: #008080;"> 73</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">conditional_expression</span><span style="color: #0000ff;">&gt;</span> ::= <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">logical_or_expression</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ;
</span><span style="color: #008080;"> 74</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">conditional_expression</span><span style="color: #0000ff;">&gt;</span> ::= <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">logical_or_expression</span><span style="color: #0000ff;">&gt;</span> "?" <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">expression</span><span style="color: #0000ff;">&gt;</span> ":" <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">assignment_expression</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ;
</span><span style="color: #008080;"> 75</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">assignment_expression</span><span style="color: #0000ff;">&gt;</span> ::= <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">conditional_expression</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ;
</span><span style="color: #008080;"> 76</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">assignment_expression</span><span style="color: #0000ff;">&gt;</span> ::= <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">unary_expression</span><span style="color: #0000ff;">&gt;</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">assignment_operator</span><span style="color: #0000ff;">&gt;</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">assignment_expression</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ;
</span><span style="color: #008080;"> 77</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">assignment_operator</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "=" ;
</span><span style="color: #008080;"> 78</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">assignment_operator</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "*=" ;
</span><span style="color: #008080;"> 79</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">assignment_operator</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "/=" ;
</span><span style="color: #008080;"> 80</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">assignment_operator</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "%=" ;
</span><span style="color: #008080;"> 81</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">assignment_operator</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "+=" ;
</span><span style="color: #008080;"> 82</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">assignment_operator</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "-=" ;
</span><span style="color: #008080;"> 83</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">assignment_operator</span><span style="color: #0000ff;">&gt;</span> ::= "<span style="color: #0000ff;">&lt;</span><span style="color: #800000;">&lt;=" ;
</span><span style="color: #008080;"> 84</span> <span style="color: #800000;">&lt;assignment_operator</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "&gt;&gt;=" ;
</span><span style="color: #008080;"> 85</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">assignment_operator</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "&amp;=" ;
</span><span style="color: #008080;"> 86</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">assignment_operator</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "^=" ;
</span><span style="color: #008080;"> 87</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">assignment_operator</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "|=" ;
</span><span style="color: #008080;"> 88</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">expression</span><span style="color: #0000ff;">&gt;</span> ::= <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">assignment_expression</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ;
</span><span style="color: #008080;"> 89</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">expression</span><span style="color: #0000ff;">&gt;</span> ::= <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">expression</span><span style="color: #0000ff;">&gt;</span> "," <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">assignment_expression</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ;
</span><span style="color: #008080;"> 90</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">constant_expression</span><span style="color: #0000ff;">&gt;</span> ::= <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">conditional_expression</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ;
</span><span style="color: #008080;"> 91</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">declaration</span><span style="color: #0000ff;">&gt;</span> ::= <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">function_prototype</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ";" ;
</span><span style="color: #008080;"> 92</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">declaration</span><span style="color: #0000ff;">&gt;</span> ::= <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">init_declarator_list</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ";" ;
</span><span style="color: #008080;"> 93</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">declaration</span><span style="color: #0000ff;">&gt;</span> ::= "precision" <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">precision_qualifier</span><span style="color: #0000ff;">&gt;</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">type_specifier</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ";" ;
</span><span style="color: #008080;"> 94</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">declaration</span><span style="color: #0000ff;">&gt;</span> ::= <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">type_qualifier</span><span style="color: #0000ff;">&gt;</span> identifier "{" <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">struct_declaration_list</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> "}" ";" ;
</span><span style="color: #008080;"> 95</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">declaration</span><span style="color: #0000ff;">&gt;</span> ::= <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">type_qualifier</span><span style="color: #0000ff;">&gt;</span> identifier "{" <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">struct_declaration_list</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> "}" identifier ";" ;
</span><span style="color: #008080;"> 96</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">declaration</span><span style="color: #0000ff;">&gt;</span> ::= <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">type_qualifier</span><span style="color: #0000ff;">&gt;</span> identifier "{" <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">struct_declaration_list</span><span style="color: #0000ff;">&gt;</span> "}" identifier <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">array_specifier</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ";" ;
</span><span style="color: #008080;"> 97</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">declaration</span><span style="color: #0000ff;">&gt;</span> ::= <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">type_qualifier</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ";" ;
</span><span style="color: #008080;"> 98</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">declaration</span><span style="color: #0000ff;">&gt;</span> ::= <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">type_qualifier</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> identifier ";" ;
</span><span style="color: #008080;"> 99</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">declaration</span><span style="color: #0000ff;">&gt;</span> ::= <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">type_qualifier</span><span style="color: #0000ff;">&gt;</span> identifier <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">identifier_list</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ";" ;
</span><span style="color: #008080;">100</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">identifier_list</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "," identifier ;
</span><span style="color: #008080;">101</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">identifier_list</span><span style="color: #0000ff;">&gt;</span> ::= <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">identifier_list</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> "," identifier ;
</span><span style="color: #008080;">102</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">function_prototype</span><span style="color: #0000ff;">&gt;</span> ::= <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">function_declarator</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ")" ;
</span><span style="color: #008080;">103</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">function_declarator</span><span style="color: #0000ff;">&gt;</span> ::= <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">function_header</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ;
</span><span style="color: #008080;">104</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">function_declarator</span><span style="color: #0000ff;">&gt;</span> ::= <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">function_header_with_parameters</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ;
</span><span style="color: #008080;">105</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">function_header_with_parameters</span><span style="color: #0000ff;">&gt;</span> ::= <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">function_header</span><span style="color: #0000ff;">&gt;</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">parameter_declaration</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ;
</span><span style="color: #008080;">106</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">function_header_with_parameters</span><span style="color: #0000ff;">&gt;</span> ::= <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">function_header_with_parameters</span><span style="color: #0000ff;">&gt;</span> "," <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">parameter_declaration</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ;
</span><span style="color: #008080;">107</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">function_header</span><span style="color: #0000ff;">&gt;</span> ::= <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">fully_specified_type</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> identifier "(" ;
</span><span style="color: #008080;">108</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">parameter_declarator</span><span style="color: #0000ff;">&gt;</span> ::= <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">type_specifier</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> identifier ;
</span><span style="color: #008080;">109</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">parameter_declarator</span><span style="color: #0000ff;">&gt;</span> ::= <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">type_specifier</span><span style="color: #0000ff;">&gt;</span> identifier <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">array_specifier</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ;
</span><span style="color: #008080;">110</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">parameter_declaration</span><span style="color: #0000ff;">&gt;</span> ::= <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">type_qualifier</span><span style="color: #0000ff;">&gt;</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">parameter_declarator</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ;
</span><span style="color: #008080;">111</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">parameter_declaration</span><span style="color: #0000ff;">&gt;</span> ::= <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">parameter_declarator</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ;
</span><span style="color: #008080;">112</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">parameter_declaration</span><span style="color: #0000ff;">&gt;</span> ::= <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">type_qualifier</span><span style="color: #0000ff;">&gt;</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">parameter_type_specifier</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ;
</span><span style="color: #008080;">113</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">parameter_declaration</span><span style="color: #0000ff;">&gt;</span> ::= <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">parameter_type_specifier</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ;
</span><span style="color: #008080;">114</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">parameter_type_specifier</span><span style="color: #0000ff;">&gt;</span> ::= <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">type_specifier</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ;
</span><span style="color: #008080;">115</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">init_declarator_list</span><span style="color: #0000ff;">&gt;</span> ::= <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">single_declaration</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ;
</span><span style="color: #008080;">116</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">init_declarator_list</span><span style="color: #0000ff;">&gt;</span> ::= <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">init_declarator_list</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> "," identifier ;
</span><span style="color: #008080;">117</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">init_declarator_list</span><span style="color: #0000ff;">&gt;</span> ::= <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">init_declarator_list</span><span style="color: #0000ff;">&gt;</span> "," identifier <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">array_specifier</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ;
</span><span style="color: #008080;">118</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">init_declarator_list</span><span style="color: #0000ff;">&gt;</span> ::= <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">init_declarator_list</span><span style="color: #0000ff;">&gt;</span> "," identifier <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">array_specifier</span><span style="color: #0000ff;">&gt;</span> "=" <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">initializer</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ;
</span><span style="color: #008080;">119</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">init_declarator_list</span><span style="color: #0000ff;">&gt;</span> ::= <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">init_declarator_list</span><span style="color: #0000ff;">&gt;</span> "," identifier "=" <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">initializer</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ;
</span><span style="color: #008080;">120</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">single_declaration</span><span style="color: #0000ff;">&gt;</span> ::= <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">fully_specified_type</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ;
</span><span style="color: #008080;">121</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">single_declaration</span><span style="color: #0000ff;">&gt;</span> ::= <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">fully_specified_type</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> identifier ;
</span><span style="color: #008080;">122</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">single_declaration</span><span style="color: #0000ff;">&gt;</span> ::= <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">fully_specified_type</span><span style="color: #0000ff;">&gt;</span> identifier <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">array_specifier</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ;
</span><span style="color: #008080;">123</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">single_declaration</span><span style="color: #0000ff;">&gt;</span> ::= <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">fully_specified_type</span><span style="color: #0000ff;">&gt;</span> identifier <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">array_specifier</span><span style="color: #0000ff;">&gt;</span> "=" <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">initializer</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ;
</span><span style="color: #008080;">124</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">single_declaration</span><span style="color: #0000ff;">&gt;</span> ::= <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">fully_specified_type</span><span style="color: #0000ff;">&gt;</span> identifier "=" <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">initializer</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ;
</span><span style="color: #008080;">125</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">fully_specified_type</span><span style="color: #0000ff;">&gt;</span> ::= <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">type_specifier</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ;
</span><span style="color: #008080;">126</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">fully_specified_type</span><span style="color: #0000ff;">&gt;</span> ::= <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">type_qualifier</span><span style="color: #0000ff;">&gt;</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">type_specifier</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ;
</span><span style="color: #008080;">127</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">invariant_qualifier</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "invariant" ;
</span><span style="color: #008080;">128</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">interpolation_qualifier</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "smooth" ;
</span><span style="color: #008080;">129</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">interpolation_qualifier</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "flat" ;
</span><span style="color: #008080;">130</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">interpolation_qualifier</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "noperspective" ;
</span><span style="color: #008080;">131</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">layout_qualifier</span><span style="color: #0000ff;">&gt;</span> ::= "layout" "(" <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">layout_qualifier_id_list</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ")" ;
</span><span style="color: #008080;">132</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">layout_qualifier_id_list</span><span style="color: #0000ff;">&gt;</span> ::= <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">layout_qualifier_id</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ;
</span><span style="color: #008080;">133</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">layout_qualifier_id_list</span><span style="color: #0000ff;">&gt;</span> ::= <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">layout_qualifier_id_list</span><span style="color: #0000ff;">&gt;</span> "," <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">layout_qualifier_id</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ;
</span><span style="color: #008080;">134</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">layout_qualifier_id</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= identifier ;
</span><span style="color: #008080;">135</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">layout_qualifier_id</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= identifier "=" number ;
</span><span style="color: #008080;">136</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">precise_qualifier</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "precise" ;
</span><span style="color: #008080;">137</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">type_qualifier</span><span style="color: #0000ff;">&gt;</span> ::= <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">single_type_qualifier</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ;
</span><span style="color: #008080;">138</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">type_qualifier</span><span style="color: #0000ff;">&gt;</span> ::= <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">type_qualifier</span><span style="color: #0000ff;">&gt;</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">single_type_qualifier</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ;
</span><span style="color: #008080;">139</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">single_type_qualifier</span><span style="color: #0000ff;">&gt;</span> ::= <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">storage_qualifier</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ;
</span><span style="color: #008080;">140</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">single_type_qualifier</span><span style="color: #0000ff;">&gt;</span> ::= <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">layout_qualifier</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ;
</span><span style="color: #008080;">141</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">single_type_qualifier</span><span style="color: #0000ff;">&gt;</span> ::= <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">precision_qualifier</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ;
</span><span style="color: #008080;">142</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">single_type_qualifier</span><span style="color: #0000ff;">&gt;</span> ::= <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">interpolation_qualifier</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ;
</span><span style="color: #008080;">143</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">single_type_qualifier</span><span style="color: #0000ff;">&gt;</span> ::= <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">invariant_qualifier</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ;
</span><span style="color: #008080;">144</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">single_type_qualifier</span><span style="color: #0000ff;">&gt;</span> ::= <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">precise_qualifier</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ;
</span><span style="color: #008080;">145</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">storage_qualifier</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "const" ;
</span><span style="color: #008080;">146</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">storage_qualifier</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "inout" ;
</span><span style="color: #008080;">147</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">storage_qualifier</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "in" ;
</span><span style="color: #008080;">148</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">storage_qualifier</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "out" ;
</span><span style="color: #008080;">149</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">storage_qualifier</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "centroid" ;
</span><span style="color: #008080;">150</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">storage_qualifier</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "patch" ;
</span><span style="color: #008080;">151</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">storage_qualifier</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "sample" ;
</span><span style="color: #008080;">152</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">storage_qualifier</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "uniform" ;
</span><span style="color: #008080;">153</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">storage_qualifier</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "buffer" ;
</span><span style="color: #008080;">154</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">storage_qualifier</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "shared" ;
</span><span style="color: #008080;">155</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">storage_qualifier</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "coherent" ;
</span><span style="color: #008080;">156</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">storage_qualifier</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "volatile" ;
</span><span style="color: #008080;">157</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">storage_qualifier</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "restrict" ;
</span><span style="color: #008080;">158</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">storage_qualifier</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "readonly" ;
</span><span style="color: #008080;">159</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">storage_qualifier</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "writeonly" ;
</span><span style="color: #008080;">160</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">storage_qualifier</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "subroutine" ;
</span><span style="color: #008080;">161</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">storage_qualifier</span><span style="color: #0000ff;">&gt;</span> ::= "subroutine" "(" <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">type_name_list</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ")" ;
</span><span style="color: #008080;">162</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">type_name_list</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= userDefinedType ;
</span><span style="color: #008080;">163</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">type_name_list</span><span style="color: #0000ff;">&gt;</span> ::= <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">type_name_list</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> "," userDefinedType ;
</span><span style="color: #008080;">164</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">type_specifier</span><span style="color: #0000ff;">&gt;</span> ::= <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">type_specifier_nonarray</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ;
</span><span style="color: #008080;">165</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">type_specifier</span><span style="color: #0000ff;">&gt;</span> ::= <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">type_specifier_nonarray</span><span style="color: #0000ff;">&gt;</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">array_specifier</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ;
</span><span style="color: #008080;">166</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">array_specifier</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "[" "]" ;
</span><span style="color: #008080;">167</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">array_specifier</span><span style="color: #0000ff;">&gt;</span> ::= "[" <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">constant_expression</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> "]" ;
</span><span style="color: #008080;">168</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">array_specifier</span><span style="color: #0000ff;">&gt;</span> ::= <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">array_specifier</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> "[" "]" ;
</span><span style="color: #008080;">169</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">array_specifier</span><span style="color: #0000ff;">&gt;</span> ::= <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">array_specifier</span><span style="color: #0000ff;">&gt;</span> "[" <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">constant_expression</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> "]" ;
</span><span style="color: #008080;">170</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">type_specifier_nonarray</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "void" ;
</span><span style="color: #008080;">171</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">type_specifier_nonarray</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "float" ;
</span><span style="color: #008080;">172</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">type_specifier_nonarray</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "double" ;
</span><span style="color: #008080;">173</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">type_specifier_nonarray</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "int" ;
</span><span style="color: #008080;">174</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">type_specifier_nonarray</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "uint" ;
</span><span style="color: #008080;">175</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">type_specifier_nonarray</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "bool" ;
</span><span style="color: #008080;">176</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">type_specifier_nonarray</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "vec2" ;
</span><span style="color: #008080;">177</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">type_specifier_nonarray</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "vec3" ;
</span><span style="color: #008080;">178</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">type_specifier_nonarray</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "vec4" ;
</span><span style="color: #008080;">179</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">type_specifier_nonarray</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "dvec2" ;
</span><span style="color: #008080;">180</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">type_specifier_nonarray</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "dvec3" ;
</span><span style="color: #008080;">181</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">type_specifier_nonarray</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "dvec4" ;
</span><span style="color: #008080;">182</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">type_specifier_nonarray</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "bvec2" ;
</span><span style="color: #008080;">183</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">type_specifier_nonarray</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "bvec3" ;
</span><span style="color: #008080;">184</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">type_specifier_nonarray</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "bvec4" ;
</span><span style="color: #008080;">185</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">type_specifier_nonarray</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "ivec2" ;
</span><span style="color: #008080;">186</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">type_specifier_nonarray</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "ivec3" ;
</span><span style="color: #008080;">187</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">type_specifier_nonarray</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "ivec4" ;
</span><span style="color: #008080;">188</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">type_specifier_nonarray</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "uvec2" ;
</span><span style="color: #008080;">189</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">type_specifier_nonarray</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "uvec3" ;
</span><span style="color: #008080;">190</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">type_specifier_nonarray</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "uvec4" ;
</span><span style="color: #008080;">191</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">type_specifier_nonarray</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "mat2" ;
</span><span style="color: #008080;">192</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">type_specifier_nonarray</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "mat3" ;
</span><span style="color: #008080;">193</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">type_specifier_nonarray</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "mat4" ;
</span><span style="color: #008080;">194</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">type_specifier_nonarray</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "mat2x2" ;
</span><span style="color: #008080;">195</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">type_specifier_nonarray</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "mat2x3" ;
</span><span style="color: #008080;">196</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">type_specifier_nonarray</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "mat2x4" ;
</span><span style="color: #008080;">197</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">type_specifier_nonarray</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "mat3x2" ;
</span><span style="color: #008080;">198</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">type_specifier_nonarray</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "mat3x3" ;
</span><span style="color: #008080;">199</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">type_specifier_nonarray</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "mat3x4" ;
</span><span style="color: #008080;">200</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">type_specifier_nonarray</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "mat4x2" ;
</span><span style="color: #008080;">201</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">type_specifier_nonarray</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "mat4x3" ;
</span><span style="color: #008080;">202</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">type_specifier_nonarray</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "mat4x4" ;
</span><span style="color: #008080;">203</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">type_specifier_nonarray</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "dmat2" ;
</span><span style="color: #008080;">204</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">type_specifier_nonarray</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "dmat3" ;
</span><span style="color: #008080;">205</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">type_specifier_nonarray</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "dmat4" ;
</span><span style="color: #008080;">206</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">type_specifier_nonarray</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "dmat2x2" ;
</span><span style="color: #008080;">207</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">type_specifier_nonarray</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "dmat2x3" ;
</span><span style="color: #008080;">208</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">type_specifier_nonarray</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "dmat2x4" ;
</span><span style="color: #008080;">209</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">type_specifier_nonarray</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "dmat3x2" ;
</span><span style="color: #008080;">210</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">type_specifier_nonarray</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "dmat3x3" ;
</span><span style="color: #008080;">211</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">type_specifier_nonarray</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "dmat3x4" ;
</span><span style="color: #008080;">212</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">type_specifier_nonarray</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "dmat4x2" ;
</span><span style="color: #008080;">213</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">type_specifier_nonarray</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "dmat4x3" ;
</span><span style="color: #008080;">214</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">type_specifier_nonarray</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "dmat4x4" ;
</span><span style="color: #008080;">215</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">type_specifier_nonarray</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "atomic_uint" ;
</span><span style="color: #008080;">216</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">type_specifier_nonarray</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "sampler1D" ;
</span><span style="color: #008080;">217</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">type_specifier_nonarray</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "sampler2D" ;
</span><span style="color: #008080;">218</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">type_specifier_nonarray</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "sampler3D" ;
</span><span style="color: #008080;">219</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">type_specifier_nonarray</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "samplerCube" ;
</span><span style="color: #008080;">220</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">type_specifier_nonarray</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "sampler1DShadow" ;
</span><span style="color: #008080;">221</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">type_specifier_nonarray</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "sampler2DShadow" ;
</span><span style="color: #008080;">222</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">type_specifier_nonarray</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "samplerCubeShadow" ;
</span><span style="color: #008080;">223</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">type_specifier_nonarray</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "sampler1DArray" ;
</span><span style="color: #008080;">224</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">type_specifier_nonarray</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "sampler2DArray" ;
</span><span style="color: #008080;">225</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">type_specifier_nonarray</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "sampler1DArrayShadow" ;
</span><span style="color: #008080;">226</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">type_specifier_nonarray</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "sampler2DArrayShadow" ;
</span><span style="color: #008080;">227</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">type_specifier_nonarray</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "samplerCubeArray" ;
</span><span style="color: #008080;">228</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">type_specifier_nonarray</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "samplerCubeArrayShadow" ;
</span><span style="color: #008080;">229</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">type_specifier_nonarray</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "isampler1D" ;
</span><span style="color: #008080;">230</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">type_specifier_nonarray</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "isampler2D" ;
</span><span style="color: #008080;">231</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">type_specifier_nonarray</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "isampler3D" ;
</span><span style="color: #008080;">232</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">type_specifier_nonarray</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "isamplerCube" ;
</span><span style="color: #008080;">233</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">type_specifier_nonarray</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "isampler1DArray" ;
</span><span style="color: #008080;">234</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">type_specifier_nonarray</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "isampler2DArray" ;
</span><span style="color: #008080;">235</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">type_specifier_nonarray</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "isamplerCubeArray" ;
</span><span style="color: #008080;">236</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">type_specifier_nonarray</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "usampler1D" ;
</span><span style="color: #008080;">237</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">type_specifier_nonarray</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "usampler2D" ;
</span><span style="color: #008080;">238</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">type_specifier_nonarray</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "usampler3D" ;
</span><span style="color: #008080;">239</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">type_specifier_nonarray</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "usamplerCube" ;
</span><span style="color: #008080;">240</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">type_specifier_nonarray</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "usampler1DArray" ;
</span><span style="color: #008080;">241</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">type_specifier_nonarray</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "usampler2DArray" ;
</span><span style="color: #008080;">242</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">type_specifier_nonarray</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "usamplerCubeArray" ;
</span><span style="color: #008080;">243</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">type_specifier_nonarray</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "sampler2DRect" ;
</span><span style="color: #008080;">244</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">type_specifier_nonarray</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "sampler2DRectShadow" ;
</span><span style="color: #008080;">245</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">type_specifier_nonarray</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "isampler2DRect" ;
</span><span style="color: #008080;">246</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">type_specifier_nonarray</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "usampler2DRect" ;
</span><span style="color: #008080;">247</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">type_specifier_nonarray</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "samplerBuffer" ;
</span><span style="color: #008080;">248</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">type_specifier_nonarray</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "isamplerBuffer" ;
</span><span style="color: #008080;">249</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">type_specifier_nonarray</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "usamplerBuffer" ;
</span><span style="color: #008080;">250</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">type_specifier_nonarray</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "sampler2DMS" ;
</span><span style="color: #008080;">251</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">type_specifier_nonarray</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "isampler2DMS" ;
</span><span style="color: #008080;">252</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">type_specifier_nonarray</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "usampler2DMS" ;
</span><span style="color: #008080;">253</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">type_specifier_nonarray</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "sampler2DMSArray" ;
</span><span style="color: #008080;">254</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">type_specifier_nonarray</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "isampler2DMSArray" ;
</span><span style="color: #008080;">255</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">type_specifier_nonarray</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "usampler2DMSArray" ;
</span><span style="color: #008080;">256</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">type_specifier_nonarray</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "image1D" ;
</span><span style="color: #008080;">257</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">type_specifier_nonarray</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "iimage1D" ;
</span><span style="color: #008080;">258</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">type_specifier_nonarray</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "uimage1D" ;
</span><span style="color: #008080;">259</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">type_specifier_nonarray</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "image2D" ;
</span><span style="color: #008080;">260</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">type_specifier_nonarray</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "iimage2D" ;
</span><span style="color: #008080;">261</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">type_specifier_nonarray</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "uimage2D" ;
</span><span style="color: #008080;">262</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">type_specifier_nonarray</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "image3D" ;
</span><span style="color: #008080;">263</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">type_specifier_nonarray</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "iimage3D" ;
</span><span style="color: #008080;">264</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">type_specifier_nonarray</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "uimage3D" ;
</span><span style="color: #008080;">265</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">type_specifier_nonarray</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "image2DRect" ;
</span><span style="color: #008080;">266</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">type_specifier_nonarray</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "iimage2DRect" ;
</span><span style="color: #008080;">267</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">type_specifier_nonarray</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "uimage2DRect" ;
</span><span style="color: #008080;">268</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">type_specifier_nonarray</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "imageCube" ;
</span><span style="color: #008080;">269</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">type_specifier_nonarray</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "iimageCube" ;
</span><span style="color: #008080;">270</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">type_specifier_nonarray</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "uimageCube" ;
</span><span style="color: #008080;">271</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">type_specifier_nonarray</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "imageBuffer" ;
</span><span style="color: #008080;">272</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">type_specifier_nonarray</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "iimageBuffer" ;
</span><span style="color: #008080;">273</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">type_specifier_nonarray</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "uimageBuffer" ;
</span><span style="color: #008080;">274</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">type_specifier_nonarray</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "image1DArray" ;
</span><span style="color: #008080;">275</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">type_specifier_nonarray</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "iimage1DArray" ;
</span><span style="color: #008080;">276</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">type_specifier_nonarray</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "uimage1DArray" ;
</span><span style="color: #008080;">277</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">type_specifier_nonarray</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "image2DArray" ;
</span><span style="color: #008080;">278</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">type_specifier_nonarray</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "iimage2DArray" ;
</span><span style="color: #008080;">279</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">type_specifier_nonarray</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "uimage2DArray" ;
</span><span style="color: #008080;">280</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">type_specifier_nonarray</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "imageCubeArray" ;
</span><span style="color: #008080;">281</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">type_specifier_nonarray</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "iimageCubeArray" ;
</span><span style="color: #008080;">282</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">type_specifier_nonarray</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "uimageCubeArray" ;
</span><span style="color: #008080;">283</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">type_specifier_nonarray</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "image2DMS" ;
</span><span style="color: #008080;">284</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">type_specifier_nonarray</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "iimage2DMS" ;
</span><span style="color: #008080;">285</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">type_specifier_nonarray</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "uimage2DMS" ;
</span><span style="color: #008080;">286</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">type_specifier_nonarray</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "image2DMSArray" ;
</span><span style="color: #008080;">287</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">type_specifier_nonarray</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "iimage2DMSArray" ;
</span><span style="color: #008080;">288</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">type_specifier_nonarray</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "uimage2DMSArray" ;
</span><span style="color: #008080;">289</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">type_specifier_nonarray</span><span style="color: #0000ff;">&gt;</span> ::= <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">struct_specifier</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ;
</span><span style="color: #008080;">290</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">type_specifier_nonarray</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= userDefinedType ;
</span><span style="color: #008080;">291</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">precision_qualifier</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "high_precision" ;
</span><span style="color: #008080;">292</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">precision_qualifier</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "medium_precision" ;
</span><span style="color: #008080;">293</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">precision_qualifier</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "low_precision" ;
</span><span style="color: #008080;">294</span> <span style="color: #000000;">// semantic parsing needed
</span><span style="color: #008080;">295</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">struct_specifier</span><span style="color: #0000ff;">&gt;</span> ::= "struct" identifier "{" <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">struct_declaration_list</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> "}" ;
</span><span style="color: #008080;">296</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">struct_specifier</span><span style="color: #0000ff;">&gt;</span> ::= "struct" "{" <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">struct_declaration_list</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> "}" ;
</span><span style="color: #008080;">297</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">struct_declaration_list</span><span style="color: #0000ff;">&gt;</span> ::= <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">struct_declaration</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ;
</span><span style="color: #008080;">298</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">struct_declaration_list</span><span style="color: #0000ff;">&gt;</span> ::= <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">struct_declaration_list</span><span style="color: #0000ff;">&gt;</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">struct_declaration</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ;
</span><span style="color: #008080;">299</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">struct_declaration</span><span style="color: #0000ff;">&gt;</span> ::= <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">type_specifier</span><span style="color: #0000ff;">&gt;</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">struct_declarator_list</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ";" ;
</span><span style="color: #008080;">300</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">struct_declaration</span><span style="color: #0000ff;">&gt;</span> ::= <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">type_qualifier</span><span style="color: #0000ff;">&gt;</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">type_specifier</span><span style="color: #0000ff;">&gt;</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">struct_declarator_list</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ";" ;
</span><span style="color: #008080;">301</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">struct_declarator_list</span><span style="color: #0000ff;">&gt;</span> ::= <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">struct_declarator</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ;
</span><span style="color: #008080;">302</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">struct_declarator_list</span><span style="color: #0000ff;">&gt;</span> ::= <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">struct_declarator_list</span><span style="color: #0000ff;">&gt;</span> "," <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">struct_declarator</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ;
</span><span style="color: #008080;">303</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">struct_declarator</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= identifier ;
</span><span style="color: #008080;">304</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">struct_declarator</span><span style="color: #0000ff;">&gt;</span> ::= identifier <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">array_specifier</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ;
</span><span style="color: #008080;">305</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">initializer</span><span style="color: #0000ff;">&gt;</span> ::= <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">assignment_expression</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ;
</span><span style="color: #008080;">306</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">initializer</span><span style="color: #0000ff;">&gt;</span> ::= "{" <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">initializer_list</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> "}" ;
</span><span style="color: #008080;">307</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">initializer</span><span style="color: #0000ff;">&gt;</span> ::= "{" <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">initializer_list</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> "," "}" ;
</span><span style="color: #008080;">308</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">initializer_list</span><span style="color: #0000ff;">&gt;</span> ::= <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">initializer</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ;
</span><span style="color: #008080;">309</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">initializer_list</span><span style="color: #0000ff;">&gt;</span> ::= <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">initializer_list</span><span style="color: #0000ff;">&gt;</span> "," <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">initializer</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ;
</span><span style="color: #008080;">310</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">declaration_statement</span><span style="color: #0000ff;">&gt;</span> ::= <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">declaration</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ;
</span><span style="color: #008080;">311</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">statement</span><span style="color: #0000ff;">&gt;</span> ::= <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">compound_statement</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ;
</span><span style="color: #008080;">312</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">statement</span><span style="color: #0000ff;">&gt;</span> ::= <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">simple_statement</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ;
</span><span style="color: #008080;">313</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">simple_statement</span><span style="color: #0000ff;">&gt;</span> ::= <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">declaration_statement</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ;
</span><span style="color: #008080;">314</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">simple_statement</span><span style="color: #0000ff;">&gt;</span> ::= <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">expression_statement</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ;
</span><span style="color: #008080;">315</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">simple_statement</span><span style="color: #0000ff;">&gt;</span> ::= <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">selection_statement</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ;
</span><span style="color: #008080;">316</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">simple_statement</span><span style="color: #0000ff;">&gt;</span> ::= <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">switch_statement</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ;
</span><span style="color: #008080;">317</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">simple_statement</span><span style="color: #0000ff;">&gt;</span> ::= <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">case_label</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ;
</span><span style="color: #008080;">318</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">simple_statement</span><span style="color: #0000ff;">&gt;</span> ::= <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">iteration_statement</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ;
</span><span style="color: #008080;">319</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">simple_statement</span><span style="color: #0000ff;">&gt;</span> ::= <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">jump_statement</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ;
</span><span style="color: #008080;">320</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">compound_statement</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "{" "}" ;
</span><span style="color: #008080;">321</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">compound_statement</span><span style="color: #0000ff;">&gt;</span> ::= "{" <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">statement_list</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> "}" ;
</span><span style="color: #008080;">322</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">statement_no_new_scope</span><span style="color: #0000ff;">&gt;</span> ::= <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">compound_statement_no_new_scope</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ;
</span><span style="color: #008080;">323</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">statement_no_new_scope</span><span style="color: #0000ff;">&gt;</span> ::= <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">simple_statement</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ;
</span><span style="color: #008080;">324</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">compound_statement_no_new_scope</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "{" "}" ;
</span><span style="color: #008080;">325</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">compound_statement_no_new_scope</span><span style="color: #0000ff;">&gt;</span> ::= "{" <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">statement_list</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> "}" ;
</span><span style="color: #008080;">326</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">statement_list</span><span style="color: #0000ff;">&gt;</span> ::= <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">statement</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ;
</span><span style="color: #008080;">327</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">statement_list</span><span style="color: #0000ff;">&gt;</span> ::= <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">statement_list</span><span style="color: #0000ff;">&gt;</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">statement</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ;
</span><span style="color: #008080;">328</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">expression_statement</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= ";" ;
</span><span style="color: #008080;">329</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">expression_statement</span><span style="color: #0000ff;">&gt;</span> ::= <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">expression</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ";" ;
</span><span style="color: #008080;">330</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">selection_statement</span><span style="color: #0000ff;">&gt;</span> ::= "if" "(" <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">expression</span><span style="color: #0000ff;">&gt;</span> ")" <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">selection_rest_statement</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ;
</span><span style="color: #008080;">331</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">selection_rest_statement</span><span style="color: #0000ff;">&gt;</span> ::= <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">statement</span><span style="color: #0000ff;">&gt;</span> "else" <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">statement</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ;
</span><span style="color: #008080;">332</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">selection_rest_statement</span><span style="color: #0000ff;">&gt;</span> ::= <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">statement</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ;
</span><span style="color: #008080;">333</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">condition</span><span style="color: #0000ff;">&gt;</span> ::= <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">expression</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ;
</span><span style="color: #008080;">334</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">condition</span><span style="color: #0000ff;">&gt;</span> ::= <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">fully_specified_type</span><span style="color: #0000ff;">&gt;</span> identifier "=" <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">initializer</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ;
</span><span style="color: #008080;">335</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">switch_statement</span><span style="color: #0000ff;">&gt;</span> ::= "switch" "(" <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">expression</span><span style="color: #0000ff;">&gt;</span> ")" "{" <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">switch_statement_list</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> "}" ;
</span><span style="color: #008080;">336</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">switch_statement_list</span><span style="color: #0000ff;">&gt;</span> ::= <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">statement_list</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ;
</span><span style="color: #008080;">337</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">case_label</span><span style="color: #0000ff;">&gt;</span> ::= "case" <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">expression</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ":" ;
</span><span style="color: #008080;">338</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">case_label</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "default" ":" ;
</span><span style="color: #008080;">339</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">iteration_statement</span><span style="color: #0000ff;">&gt;</span> ::= "while" "(" <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">condition</span><span style="color: #0000ff;">&gt;</span> ")" <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">statement_no_new_scope</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ;
</span><span style="color: #008080;">340</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">iteration_statement</span><span style="color: #0000ff;">&gt;</span> ::= "do" <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">statement</span><span style="color: #0000ff;">&gt;</span> "while" "(" <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">expression</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ")" ";" ;
</span><span style="color: #008080;">341</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">iteration_statement</span><span style="color: #0000ff;">&gt;</span> ::= "for" "(" <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">for_init_statement</span><span style="color: #0000ff;">&gt;</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">for_rest_statement</span><span style="color: #0000ff;">&gt;</span> ")" <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">statement_no_new_scope</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ;
</span><span style="color: #008080;">342</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">for_init_statement</span><span style="color: #0000ff;">&gt;</span> ::= <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">expression_statement</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ;
</span><span style="color: #008080;">343</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">for_init_statement</span><span style="color: #0000ff;">&gt;</span> ::= <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">declaration_statement</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ;
</span><span style="color: #008080;">344</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">conditionopt</span><span style="color: #0000ff;">&gt;</span> ::= <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">condition</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ;
</span><span style="color: #008080;">345</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">for_rest_statement</span><span style="color: #0000ff;">&gt;</span> ::= <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">conditionopt</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ";" ;
</span><span style="color: #008080;">346</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">for_rest_statement</span><span style="color: #0000ff;">&gt;</span> ::= <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">conditionopt</span><span style="color: #0000ff;">&gt;</span> ";" <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">expression</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ;
</span><span style="color: #008080;">347</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">jump_statement</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "continue" ";" ;
</span><span style="color: #008080;">348</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">jump_statement</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "break" ";" ;
</span><span style="color: #008080;">349</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">jump_statement</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ::= "return" ";" ;
</span><span style="color: #008080;">350</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">jump_statement</span><span style="color: #0000ff;">&gt;</span> ::= "return" <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">expression</span><span style="color: #0000ff;">&gt;</span><span style="color: #000000;"> ";" ;
</span><span style="color: #008080;">351</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">jump_statement</span><span style="color: #0000ff;">&gt;</span> ::= "discard" ";" ;</pre>
</div>
<span class="cnblogs_code_collapse">GLSL.Grammar</span></div>
<h2>补充语义分析片段</h2>
<p>语义分析是不能自动生成的。此时需要的语义分析，只有<span style="color: red;">找到自定义类型</span>这一个目的。</p>
<p>在GLSL文法里，是下面这个state需要进行语义分析。此时，分析器刚刚读到用户自定义的类型名字（<span style="color: #ff0000;">identifier</span>）。</p>
<div class="cnblogs_code">
<pre><span style="color: #008080;">1</span> <span style="color: #000000;">State [172]:
</span><span style="color: #008080;">2</span> <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">struct_specifier</span><span style="color: #0000ff;">&gt;</span> ::= "struct" <span style="color: #ff0000;">identifier</span> . "{" <span style="color: #0000ff;">&lt;</span><span style="color: #800000;">struct_declaration_list</span><span style="color: #0000ff;">&gt;</span> "}" ;, identifier "," ")" "(" ";" "["</pre>
</div>
<p>语义分析动作内容则十分简单，将identifier的内容作为自定义类型名加入UserDefinedTypeTable即可。</p>
<div class="cnblogs_code">
<pre><span style="color: #008080;">1</span>         <span style="color: #008000;">//</span><span style="color: #008000;"> State [172]:
</span><span style="color: #008080;">2</span>         <span style="color: #008000;">//</span><span style="color: #008000;"> &lt;struct_specifier&gt; ::= "struct" identifier . "{" &lt;struct_declaration_list&gt; "}" ;, identifier "," ")" "(" ";" "["</span>
<span style="color: #008080;">3</span>         <span style="color: #0000ff;">static</span> <span style="color: #0000ff;">void</span><span style="color: #000000;"> state172_struct_specifier(ParsingStepContext context)
</span><span style="color: #008080;">4</span> <span style="color: #000000;">        {
</span><span style="color: #008080;">5</span>             SyntaxTree tree =<span style="color: #000000;"> context.TreeStack.Peek();
</span><span style="color: #008080;">6</span>             <span style="color: #0000ff;">string</span> name =<span style="color: #000000;"> tree.NodeType.Content;
</span><span style="color: #008080;">7</span>             context.UserDefinedTypeTable.TryInsert(<span style="color: #0000ff;">new</span><span style="color: #000000;"> UserDefinedType(name));
</span><span style="color: #008080;">8</span>         }</pre>
</div>
<p>当然，别忘了在初始化时将此动作绑定到对应的state上。</p>
<div class="cnblogs_code">
<pre><span style="color: #008080;"> 1</span>         <span style="color: #0000ff;">static</span><span style="color: #000000;"> GLSLSyntaxParser()
</span><span style="color: #008080;"> 2</span> <span style="color: #000000;">        {
</span><span style="color: #008080;"> 3</span>             <span style="color: #008000;">//</span><span style="color: #008000;"> 将语义动作绑定的到state上。</span>
<span style="color: #008080;"> 4</span>             dict.Add(<span style="color: #0000ff;">new</span> LR1ShiftInAction(<span style="color: #800080;">172</span><span style="color: #000000;">), state172_struct_specifier);
</span><span style="color: #008080;"> 5</span> <span style="color: #000000;">        }
</span><span style="color: #008080;"> 6</span>        <span style="color: #0000ff;">static</span> Dictionary&lt;LRParsingAction, Action&lt;ParsingStepContext&gt;&gt; dict =
<span style="color: #008080;"> 7</span>             <span style="color: #0000ff;">new</span> Dictionary&lt;LRParsingAction, Action&lt;ParsingStepContext&gt;&gt;<span style="color: #000000;">();
</span><span style="color: #008080;"> 8</span> 
<span style="color: #008080;"> 9</span>         <span style="color: #0000ff;">protected</span> <span style="color: #0000ff;">override</span> Action&lt;ParsingStepContext&gt;<span style="color: #000000;"> GetSemanticAction(LRParsingAction parsingAction)
</span><span style="color: #008080;">10</span> <span style="color: #000000;">        {
</span><span style="color: #008080;">11</span>             Action&lt;ParsingStepContext&gt; semanticAction = <span style="color: #0000ff;">null</span><span style="color: #000000;">;
</span><span style="color: #008080;">12</span>             <span style="color: #0000ff;">if</span> (dict.TryGetValue(parsingAction, <span style="color: #0000ff;">out</span><span style="color: #000000;"> semanticAction))
</span><span style="color: #008080;">13</span> <span style="color: #000000;">            {
</span><span style="color: #008080;">14</span>                 <span style="color: #0000ff;">return</span><span style="color: #000000;"> semanticAction;
</span><span style="color: #008080;">15</span> <span style="color: #000000;">            }
</span><span style="color: #008080;">16</span>             <span style="color: #0000ff;">else</span>
<span style="color: #008080;">17</span> <span style="color: #000000;">            {
</span><span style="color: #008080;">18</span>                 <span style="color: #0000ff;">return</span> <span style="color: #0000ff;">null</span><span style="color: #000000;">;
</span><span style="color: #008080;">19</span> <span style="color: #000000;">            }
</span><span style="color: #008080;">20</span>         }</pre>
</div>
<h2>userDefinedType</h2>
<p>下面是上文的LightInfo代码片段的词法分析结果。请注意在定义LightInfo时，他是个identifier，定义之后，就是一个userDefinedType类型的单词了。</p>
<div class="cnblogs_code">
<pre><span style="color: #008080;"> 1</span> TokenList[Count: <span style="color: #800080;">21</span><span style="color: #000000;">]
</span><span style="color: #008080;"> 2</span> [[<span style="color: #0000ff;">struct</span>](__struct)[<span style="color: #0000ff;">struct</span>]]$[Ln:<span style="color: #800080;">1</span>, Col:<span style="color: #800080;">1</span><span style="color: #000000;">]
</span><span style="color: #008080;"> 3</span> <span style="color: #ff0000;">[[LightInfo](identifier)[LightInfo]]$[Ln:1, Col:8]
</span><span style="color: #008080;"> 4</span> [[{](__left_brace)[<span style="color: #800000;">"</span><span style="color: #800000;">{</span><span style="color: #800000;">"</span>]]$[Ln:<span style="color: #800080;">1</span>, Col:<span style="color: #800080;">18</span><span style="color: #000000;">]
</span><span style="color: #008080;"> 5</span> [[vec4](__vec4)[vec4]]$[Ln:<span style="color: #800080;">2</span>, Col:<span style="color: #800080;">5</span><span style="color: #000000;">]
</span><span style="color: #008080;"> 6</span> [[Position](identifier)[Position]]$[Ln:<span style="color: #800080;">2</span>, Col:<span style="color: #800080;">10</span><span style="color: #000000;">]
</span><span style="color: #008080;"> 7</span> [[;](__semicolon)[<span style="color: #800000;">"</span><span style="color: #800000;">;</span><span style="color: #800000;">"</span>]]$[Ln:<span style="color: #800080;">2</span>, Col:<span style="color: #800080;">18</span><span style="color: #000000;">]
</span><span style="color: #008080;"> 8</span> [[vec3](__vec3)[vec3]]$[Ln:<span style="color: #800080;">3</span>, Col:<span style="color: #800080;">5</span><span style="color: #000000;">]
</span><span style="color: #008080;"> 9</span> [[La](identifier)[La]]$[Ln:<span style="color: #800080;">3</span>, Col:<span style="color: #800080;">10</span><span style="color: #000000;">]
</span><span style="color: #008080;">10</span> [[;](__semicolon)[<span style="color: #800000;">"</span><span style="color: #800000;">;</span><span style="color: #800000;">"</span>]]$[Ln:<span style="color: #800080;">3</span>, Col:<span style="color: #800080;">12</span><span style="color: #000000;">]
</span><span style="color: #008080;">11</span> [[vec3](__vec3)[vec3]]$[Ln:<span style="color: #800080;">4</span>, Col:<span style="color: #800080;">5</span><span style="color: #000000;">]
</span><span style="color: #008080;">12</span> [[Ld](identifier)[Ld]]$[Ln:<span style="color: #800080;">4</span>, Col:<span style="color: #800080;">10</span><span style="color: #000000;">]
</span><span style="color: #008080;">13</span> [[;](__semicolon)[<span style="color: #800000;">"</span><span style="color: #800000;">;</span><span style="color: #800000;">"</span>]]$[Ln:<span style="color: #800080;">4</span>, Col:<span style="color: #800080;">12</span><span style="color: #000000;">]
</span><span style="color: #008080;">14</span> [[vec3](__vec3)[vec3]]$[Ln:<span style="color: #800080;">5</span>, Col:<span style="color: #800080;">5</span><span style="color: #000000;">]
</span><span style="color: #008080;">15</span> [[Ls](identifier)[Ls]]$[Ln:<span style="color: #800080;">5</span>, Col:<span style="color: #800080;">10</span><span style="color: #000000;">]
</span><span style="color: #008080;">16</span> [[;](__semicolon)[<span style="color: #800000;">"</span><span style="color: #800000;">;</span><span style="color: #800000;">"</span>]]$[Ln:<span style="color: #800080;">5</span>, Col:<span style="color: #800080;">12</span><span style="color: #000000;">]
</span><span style="color: #008080;">17</span> [[}](__right_brace)[<span style="color: #800000;">"</span><span style="color: #800000;">}</span><span style="color: #800000;">"</span>]]$[Ln:<span style="color: #800080;">6</span>, Col:<span style="color: #800080;">1</span><span style="color: #000000;">]
</span><span style="color: #008080;">18</span> [[;](__semicolon)[<span style="color: #800000;">"</span><span style="color: #800000;">;</span><span style="color: #800000;">"</span>]]$[Ln:<span style="color: #800080;">6</span>, Col:<span style="color: #800080;">2</span><span style="color: #000000;">]
</span><span style="color: #008080;">19</span> [[uniform](__uniform)[uniform]]$[Ln:<span style="color: #800080;">7</span>, Col:<span style="color: #800080;">1</span><span style="color: #000000;">]
</span><span style="color: #008080;">20</span> <span style="color: #ff0000;">[[LightInfo](__userDefinedType)[LightInfo]]$[Ln:7, Col:9]
</span><span style="color: #008080;">21</span> [[Light](identifier)[Light]]$[Ln:<span style="color: #800080;">7</span>, Col:<span style="color: #800080;">19</span><span style="color: #000000;">]
</span><span style="color: #008080;">22</span> [[;](__semicolon)[<span style="color: #800000;">"</span><span style="color: #800000;">;</span><span style="color: #800000;">"</span>]]$[Ln:<span style="color: #800080;">7</span>, Col:<span style="color: #800080;">24</span>]</pre>
</div>
<p>下面是LightInfo片段的语法树。你可以看到单词的类型对照着叶结点的类型。</p>
<div class="cnblogs_code" onclick="cnblogs_code_show('6330c88a-6ab3-42f5-8b1c-c81fc2013056')"><img id="code_img_closed_6330c88a-6ab3-42f5-8b1c-c81fc2013056" class="code_img_closed" src="http://images.cnblogs.com/OutliningIndicators/ContractedBlock.gif" alt="" /><img id="code_img_opened_6330c88a-6ab3-42f5-8b1c-c81fc2013056" class="code_img_opened" style="display: none;" onclick="cnblogs_code_hide('6330c88a-6ab3-42f5-8b1c-c81fc2013056',event)" src="http://images.cnblogs.com/OutliningIndicators/ExpandedBlockStart.gif" alt="" />
<div id="cnblogs_code_open_6330c88a-6ab3-42f5-8b1c-c81fc2013056" class="cnblogs_code_hide">
<pre><span style="color: #008080;"> 1</span> (__translation_unit)[&lt;translation_unit&gt;<span style="color: #000000;">][translation_unit]
</span><span style="color: #008080;"> 2</span>  ├─(__translation_unit)[&lt;translation_unit&gt;<span style="color: #000000;">][translation_unit]
</span><span style="color: #008080;"> 3</span>  │  └─(__external_declaration)[&lt;external_declaration&gt;<span style="color: #000000;">][external_declaration]
</span><span style="color: #008080;"> 4</span>  │      └─(__declaration)[&lt;declaration&gt;<span style="color: #000000;">][declaration]
</span><span style="color: #008080;"> 5</span>  │          ├─(__init_declarator_list)[&lt;init_declarator_list&gt;<span style="color: #000000;">][init_declarator_list]
</span><span style="color: #008080;"> 6</span>  │          │  └─(__single_declaration)[&lt;single_declaration&gt;<span style="color: #000000;">][single_declaration]
</span><span style="color: #008080;"> 7</span>  │          │      └─(__fully_specified_type)[&lt;fully_specified_type&gt;<span style="color: #000000;">][fully_specified_type]
</span><span style="color: #008080;"> 8</span>  │          │          └─(__type_specifier)[&lt;type_specifier&gt;<span style="color: #000000;">][type_specifier]
</span><span style="color: #008080;"> 9</span>  │          │              └─(__type_specifier_nonarray)[&lt;type_specifier_nonarray&gt;<span style="color: #000000;">][type_specifier_nonarray]
</span><span style="color: #008080;">10</span>  │          │                  └─(__struct_specifier)[&lt;struct_specifier&gt;<span style="color: #000000;">][struct_specifier]
</span><span style="color: #008080;">11</span>  │          │                      ├─(__structLeave__)[<span style="color: #0000ff;">struct</span>][<span style="color: #0000ff;">struct</span><span style="color: #000000;">]
</span><span style="color: #008080;">12</span> <span style="color: #000000;"> │          │                      ├─(identifierLeave__)[LightInfo][LightInfo]
</span><span style="color: #008080;">13</span>  │          │                      ├─(__left_braceLeave__)[<span style="color: #800000;">"</span><span style="color: #800000;">{</span><span style="color: #800000;">"</span><span style="color: #000000;">][{]
</span><span style="color: #008080;">14</span>  │          │                      ├─(__struct_declaration_list)[&lt;struct_declaration_list&gt;<span style="color: #000000;">][struct_declaration_list]
</span><span style="color: #008080;">15</span>  │          │                      │  ├─(__struct_declaration_list)[&lt;struct_declaration_list&gt;<span style="color: #000000;">][struct_declaration_list]
</span><span style="color: #008080;">16</span>  │          │                      │  │  ├─(__struct_declaration_list)[&lt;struct_declaration_list&gt;<span style="color: #000000;">][struct_declaration_list]
</span><span style="color: #008080;">17</span>  │          │                      │  │  │  ├─(__struct_declaration_list)[&lt;struct_declaration_list&gt;<span style="color: #000000;">][struct_declaration_list]
</span><span style="color: #008080;">18</span>  │          │                      │  │  │  │  └─(__struct_declaration)[&lt;struct_declaration&gt;<span style="color: #000000;">][struct_declaration]
</span><span style="color: #008080;">19</span>  │          │                      │  │  │  │      ├─(__type_specifier)[&lt;type_specifier&gt;<span style="color: #000000;">][type_specifier]
</span><span style="color: #008080;">20</span>  │          │                      │  │  │  │      │  └─(__type_specifier_nonarray)[&lt;type_specifier_nonarray&gt;<span style="color: #000000;">][type_specifier_nonarray]
</span><span style="color: #008080;">21</span> <span style="color: #000000;"> │          │                      │  │  │  │      │      └─(__vec4Leave__)[vec4][vec4]
</span><span style="color: #008080;">22</span>  │          │                      │  │  │  │      ├─(__struct_declarator_list)[&lt;struct_declarator_list&gt;<span style="color: #000000;">][struct_declarator_list]
</span><span style="color: #008080;">23</span>  │          │                      │  │  │  │      │  └─(__struct_declarator)[&lt;struct_declarator&gt;<span style="color: #000000;">][struct_declarator]
</span><span style="color: #008080;">24</span> <span style="color: #000000;"> │          │                      │  │  │  │      │      └─(identifierLeave__)[Position][Position]
</span><span style="color: #008080;">25</span>  │          │                      │  │  │  │      └─(__semicolonLeave__)[<span style="color: #800000;">"</span><span style="color: #800000;">;</span><span style="color: #800000;">"</span><span style="color: #000000;">][;]
</span><span style="color: #008080;">26</span>  │          │                      │  │  │  └─(__struct_declaration)[&lt;struct_declaration&gt;<span style="color: #000000;">][struct_declaration]
</span><span style="color: #008080;">27</span>  │          │                      │  │  │      ├─(__type_specifier)[&lt;type_specifier&gt;<span style="color: #000000;">][type_specifier]
</span><span style="color: #008080;">28</span>  │          │                      │  │  │      │  └─(__type_specifier_nonarray)[&lt;type_specifier_nonarray&gt;<span style="color: #000000;">][type_specifier_nonarray]
</span><span style="color: #008080;">29</span> <span style="color: #000000;"> │          │                      │  │  │      │      └─(__vec3Leave__)[vec3][vec3]
</span><span style="color: #008080;">30</span>  │          │                      │  │  │      ├─(__struct_declarator_list)[&lt;struct_declarator_list&gt;<span style="color: #000000;">][struct_declarator_list]
</span><span style="color: #008080;">31</span>  │          │                      │  │  │      │  └─(__struct_declarator)[&lt;struct_declarator&gt;<span style="color: #000000;">][struct_declarator]
</span><span style="color: #008080;">32</span> <span style="color: #000000;"> │          │                      │  │  │      │      └─(identifierLeave__)[La][La]
</span><span style="color: #008080;">33</span>  │          │                      │  │  │      └─(__semicolonLeave__)[<span style="color: #800000;">"</span><span style="color: #800000;">;</span><span style="color: #800000;">"</span><span style="color: #000000;">][;]
</span><span style="color: #008080;">34</span>  │          │                      │  │  └─(__struct_declaration)[&lt;struct_declaration&gt;<span style="color: #000000;">][struct_declaration]
</span><span style="color: #008080;">35</span>  │          │                      │  │      ├─(__type_specifier)[&lt;type_specifier&gt;<span style="color: #000000;">][type_specifier]
</span><span style="color: #008080;">36</span>  │          │                      │  │      │  └─(__type_specifier_nonarray)[&lt;type_specifier_nonarray&gt;<span style="color: #000000;">][type_specifier_nonarray]
</span><span style="color: #008080;">37</span> <span style="color: #000000;"> │          │                      │  │      │      └─(__vec3Leave__)[vec3][vec3]
</span><span style="color: #008080;">38</span>  │          │                      │  │      ├─(__struct_declarator_list)[&lt;struct_declarator_list&gt;<span style="color: #000000;">][struct_declarator_list]
</span><span style="color: #008080;">39</span>  │          │                      │  │      │  └─(__struct_declarator)[&lt;struct_declarator&gt;<span style="color: #000000;">][struct_declarator]
</span><span style="color: #008080;">40</span> <span style="color: #000000;"> │          │                      │  │      │      └─(identifierLeave__)[Ld][Ld]
</span><span style="color: #008080;">41</span>  │          │                      │  │      └─(__semicolonLeave__)[<span style="color: #800000;">"</span><span style="color: #800000;">;</span><span style="color: #800000;">"</span><span style="color: #000000;">][;]
</span><span style="color: #008080;">42</span>  │          │                      │  └─(__struct_declaration)[&lt;struct_declaration&gt;<span style="color: #000000;">][struct_declaration]
</span><span style="color: #008080;">43</span>  │          │                      │      ├─(__type_specifier)[&lt;type_specifier&gt;<span style="color: #000000;">][type_specifier]
</span><span style="color: #008080;">44</span>  │          │                      │      │  └─(__type_specifier_nonarray)[&lt;type_specifier_nonarray&gt;<span style="color: #000000;">][type_specifier_nonarray]
</span><span style="color: #008080;">45</span> <span style="color: #000000;"> │          │                      │      │      └─(__vec3Leave__)[vec3][vec3]
</span><span style="color: #008080;">46</span>  │          │                      │      ├─(__struct_declarator_list)[&lt;struct_declarator_list&gt;<span style="color: #000000;">][struct_declarator_list]
</span><span style="color: #008080;">47</span>  │          │                      │      │  └─(__struct_declarator)[&lt;struct_declarator&gt;<span style="color: #000000;">][struct_declarator]
</span><span style="color: #008080;">48</span> <span style="color: #000000;"> │          │                      │      │      └─(identifierLeave__)[Ls][Ls]
</span><span style="color: #008080;">49</span>  │          │                      │      └─(__semicolonLeave__)[<span style="color: #800000;">"</span><span style="color: #800000;">;</span><span style="color: #800000;">"</span><span style="color: #000000;">][;]
</span><span style="color: #008080;">50</span>  │          │                      └─(__right_braceLeave__)[<span style="color: #800000;">"</span><span style="color: #800000;">}</span><span style="color: #800000;">"</span><span style="color: #000000;">][}]
</span><span style="color: #008080;">51</span>  │          └─(__semicolonLeave__)[<span style="color: #800000;">"</span><span style="color: #800000;">;</span><span style="color: #800000;">"</span><span style="color: #000000;">][;]
</span><span style="color: #008080;">52</span>  └─(__external_declaration)[&lt;external_declaration&gt;<span style="color: #000000;">][external_declaration]
</span><span style="color: #008080;">53</span>      └─(__declaration)[&lt;declaration&gt;<span style="color: #000000;">][declaration]
</span><span style="color: #008080;">54</span>          ├─(__init_declarator_list)[&lt;init_declarator_list&gt;<span style="color: #000000;">][init_declarator_list]
</span><span style="color: #008080;">55</span>          │  └─(__single_declaration)[&lt;single_declaration&gt;<span style="color: #000000;">][single_declaration]
</span><span style="color: #008080;">56</span>          │      ├─(__fully_specified_type)[&lt;fully_specified_type&gt;<span style="color: #000000;">][fully_specified_type]
</span><span style="color: #008080;">57</span>          │      │  ├─(__type_qualifier)[&lt;type_qualifier&gt;<span style="color: #000000;">][type_qualifier]
</span><span style="color: #008080;">58</span>          │      │  │  └─(__single_type_qualifier)[&lt;single_type_qualifier&gt;<span style="color: #000000;">][single_type_qualifier]
</span><span style="color: #008080;">59</span>          │      │  │      └─(__storage_qualifier)[&lt;storage_qualifier&gt;<span style="color: #000000;">][storage_qualifier]
</span><span style="color: #008080;">60</span> <span style="color: #000000;">         │      │  │          └─(__uniformLeave__)[uniform][uniform]
</span><span style="color: #008080;">61</span>          │      │  └─(__type_specifier)[&lt;type_specifier&gt;<span style="color: #000000;">][type_specifier]
</span><span style="color: #008080;">62</span>          │      │      └─(__type_specifier_nonarray)[&lt;type_specifier_nonarray&gt;<span style="color: #000000;">][type_specifier_nonarray]
</span><span style="color: #008080;">63</span> <span style="color: #000000;">         │      │          └─(__userDefinedTypeLeave__)[LightInfo][LightInfo]
</span><span style="color: #008080;">64</span> <span style="color: #000000;">         │      └─(identifierLeave__)[Light][Light]
</span><span style="color: #008080;">65</span>          └─(__semicolonLeave__)[<span style="color: #800000;">"</span><span style="color: #800000;">;</span><span style="color: #800000;">"</span>][;]</pre>
</div>
<span class="cnblogs_code_collapse">SyntaxTree</span></div>
<p>再加上其他的测试用例，这个GLSL解析器终于实现了。</p>
<h2>最终目的</h2>
<p>解析GLSL源代码，是为了获取其中的信息（都有哪些in/out/uniform等）。现在语法树已经有了，剩下的就是遍历此树的事了。不再详述。</p>
<h1>故事</h1>
<p>故事，其实是事故。由于心急，此项目第一次实现时出现了几乎无法fix的bug。于是重写了一遍，这次一步一步走，终于成功了。</p>
<h2>LALR(1)State</h2>
<p>LALR(1)State集合在尝试插入一个新的State时，如果已有在LALR(1)意义上"相等"的状态，仍旧要尝试将新state的LookAhead列表插入已有状态。</p>
<p><img src="http://images2015.cnblogs.com/blog/383191/201604/383191-20160415230944301-914787360.png" alt="" /></p>
<p>否则，下面的例子就显示了文法3-8在忽视了这一点时的state集合与正确的state集合的差别（少了一些LookAhead项）。</p>
<div class="cnblogs_code" onclick="cnblogs_code_show('6c0bc999-7872-4f7e-9a51-41429e5678a7')"><img id="code_img_closed_6c0bc999-7872-4f7e-9a51-41429e5678a7" class="code_img_closed" src="http://images.cnblogs.com/OutliningIndicators/ContractedBlock.gif" alt="" /><img id="code_img_opened_6c0bc999-7872-4f7e-9a51-41429e5678a7" class="code_img_opened" style="display: none;" onclick="cnblogs_code_hide('6c0bc999-7872-4f7e-9a51-41429e5678a7',event)" src="http://images.cnblogs.com/OutliningIndicators/ExpandedBlockStart.gif" alt="" />
<div id="cnblogs_code_open_6c0bc999-7872-4f7e-9a51-41429e5678a7" class="cnblogs_code_hide">
<pre><span style="color: #008080;"> 1</span> State [<span style="color: #800080;">1</span><span style="color: #000000;">]:
</span><span style="color: #008080;"> 2</span> &lt;S&gt; ::= . <span style="color: #800000;">"</span><span style="color: #800000;">(</span><span style="color: #800000;">"</span> &lt;L&gt; <span style="color: #800000;">"</span><span style="color: #800000;">)</span><span style="color: #800000;">"</span> ;, <span style="color: #800000;">"</span><span style="color: #800000;">$</span><span style="color: #800000;">"</span>
<span style="color: #008080;"> 3</span> &lt;S<span style="color: #800000;">'</span><span style="color: #800000;">&gt; ::= . &lt;S&gt; "$" ;, "$"</span>
<span style="color: #008080;"> 4</span> &lt;S&gt; ::= . <span style="color: #800000;">"</span><span style="color: #800000;">x</span><span style="color: #800000;">"</span> ;, <span style="color: #800000;">"</span><span style="color: #800000;">$</span><span style="color: #800000;">"</span>
<span style="color: #008080;"> 5</span> State [<span style="color: #800080;">8</span><span style="color: #000000;">]:
</span><span style="color: #008080;"> 6</span> &lt;S&gt; ::= <span style="color: #800000;">"</span><span style="color: #800000;">(</span><span style="color: #800000;">"</span> &lt;L&gt; <span style="color: #800000;">"</span><span style="color: #800000;">)</span><span style="color: #800000;">"</span> . ;, <span style="color: #800000;">"</span><span style="color: #800000;">$</span><span style="color: #800000;">"</span>
<span style="color: #008080;"> 7</span> State [<span style="color: #800080;">4</span><span style="color: #000000;">]:
</span><span style="color: #008080;"> 8</span> &lt;S&gt; ::= <span style="color: #800000;">"</span><span style="color: #800000;">x</span><span style="color: #800000;">"</span> . ;, <span style="color: #800000;">"</span><span style="color: #800000;">$</span><span style="color: #800000;">"</span>
<span style="color: #008080;"> 9</span> State [<span style="color: #800080;">6</span><span style="color: #000000;">]:
</span><span style="color: #008080;">10</span> &lt;L&gt; ::= &lt;S&gt; . ;, <span style="color: #800000;">"</span><span style="color: #800000;">,</span><span style="color: #800000;">""</span><span style="color: #800000;">)</span><span style="color: #800000;">"</span>
<span style="color: #008080;">11</span> State [<span style="color: #800080;">9</span><span style="color: #000000;">]:
</span><span style="color: #008080;">12</span> &lt;L&gt; ::= &lt;L&gt; <span style="color: #800000;">"</span><span style="color: #800000;">,</span><span style="color: #800000;">"</span> &lt;S&gt; . ;, <span style="color: #800000;">"</span><span style="color: #800000;">,</span><span style="color: #800000;">""</span><span style="color: #800000;">)</span><span style="color: #800000;">"</span>
<span style="color: #008080;">13</span> State [<span style="color: #800080;">5</span><span style="color: #000000;">]:
</span><span style="color: #008080;">14</span> &lt;L&gt; ::= &lt;L&gt; . <span style="color: #800000;">"</span><span style="color: #800000;">,</span><span style="color: #800000;">"</span> &lt;S&gt; ;, <span style="color: #800000;">"</span><span style="color: #800000;">,</span><span style="color: #800000;">""</span><span style="color: #800000;">)</span><span style="color: #800000;">"</span>
<span style="color: #008080;">15</span> &lt;S&gt; ::= <span style="color: #800000;">"</span><span style="color: #800000;">(</span><span style="color: #800000;">"</span> &lt;L&gt; . <span style="color: #800000;">"</span><span style="color: #800000;">)</span><span style="color: #800000;">"</span> ;, <span style="color: #800000;">"</span><span style="color: #800000;">$</span><span style="color: #800000;">"</span>
<span style="color: #008080;">16</span> State [<span style="color: #800080;">7</span><span style="color: #000000;">]:
</span><span style="color: #008080;">17</span> &lt;S&gt; ::= . <span style="color: #800000;">"</span><span style="color: #800000;">(</span><span style="color: #800000;">"</span> &lt;L&gt; <span style="color: #800000;">"</span><span style="color: #800000;">)</span><span style="color: #800000;">"</span> ;, <span style="color: #800000;">"</span><span style="color: #800000;">,</span><span style="color: #800000;">""</span><span style="color: #800000;">)</span><span style="color: #800000;">"</span>
<span style="color: #008080;">18</span> &lt;S&gt; ::= . <span style="color: #800000;">"</span><span style="color: #800000;">x</span><span style="color: #800000;">"</span> ;, <span style="color: #800000;">"</span><span style="color: #800000;">,</span><span style="color: #800000;">""</span><span style="color: #800000;">)</span><span style="color: #800000;">"</span>
<span style="color: #008080;">19</span> &lt;L&gt; ::= &lt;L&gt; <span style="color: #800000;">"</span><span style="color: #800000;">,</span><span style="color: #800000;">"</span> . &lt;S&gt; ;, <span style="color: #800000;">"</span><span style="color: #800000;">,</span><span style="color: #800000;">""</span><span style="color: #800000;">)</span><span style="color: #800000;">"</span>
<span style="color: #008080;">20</span> State [<span style="color: #800080;">2</span><span style="color: #000000;">]:
</span><span style="color: #008080;">21</span> &lt;S&gt; ::= . <span style="color: #800000;">"</span><span style="color: #800000;">(</span><span style="color: #800000;">"</span> &lt;L&gt; <span style="color: #800000;">"</span><span style="color: #800000;">)</span><span style="color: #800000;">"</span> ;, <span style="color: #800000;">"</span><span style="color: #800000;">,</span><span style="color: #800000;">""</span><span style="color: #800000;">)</span><span style="color: #800000;">"</span>
<span style="color: #008080;">22</span> &lt;S&gt; ::= . <span style="color: #800000;">"</span><span style="color: #800000;">x</span><span style="color: #800000;">"</span> ;, <span style="color: #800000;">"</span><span style="color: #800000;">,</span><span style="color: #800000;">""</span><span style="color: #800000;">)</span><span style="color: #800000;">"</span>
<span style="color: #008080;">23</span> &lt;S&gt; ::= <span style="color: #800000;">"</span><span style="color: #800000;">(</span><span style="color: #800000;">"</span> . &lt;L&gt; <span style="color: #800000;">"</span><span style="color: #800000;">)</span><span style="color: #800000;">"</span> ;, <span style="color: #800000;">"</span><span style="color: #800000;">$</span><span style="color: #800000;">"</span>
<span style="color: #008080;">24</span> &lt;L&gt; ::= . &lt;L&gt; <span style="color: #800000;">"</span><span style="color: #800000;">,</span><span style="color: #800000;">"</span> &lt;S&gt; ;, <span style="color: #800000;">"</span><span style="color: #800000;">,</span><span style="color: #800000;">""</span><span style="color: #800000;">)</span><span style="color: #800000;">"</span>
<span style="color: #008080;">25</span> &lt;L&gt; ::= . &lt;S&gt; ;, <span style="color: #800000;">"</span><span style="color: #800000;">,</span><span style="color: #800000;">""</span><span style="color: #800000;">)</span><span style="color: #800000;">"</span>
<span style="color: #008080;">26</span> State [<span style="color: #800080;">3</span><span style="color: #000000;">]:
</span><span style="color: #008080;">27</span> &lt;S<span style="color: #800000;">'</span><span style="color: #800000;">&gt; ::= &lt;S&gt; . "$" ;, "$"</span></pre>
</div>
<span class="cnblogs_code_collapse">少LookAhead项的</span></div>
<div class="cnblogs_code" onclick="cnblogs_code_show('09e25cc9-4875-4b71-9038-156dc08eec94')"><img id="code_img_closed_09e25cc9-4875-4b71-9038-156dc08eec94" class="code_img_closed" src="http://images.cnblogs.com/OutliningIndicators/ContractedBlock.gif" alt="" /><img id="code_img_opened_09e25cc9-4875-4b71-9038-156dc08eec94" class="code_img_opened" style="display: none;" onclick="cnblogs_code_hide('09e25cc9-4875-4b71-9038-156dc08eec94',event)" src="http://images.cnblogs.com/OutliningIndicators/ExpandedBlockStart.gif" alt="" />
<div id="cnblogs_code_open_09e25cc9-4875-4b71-9038-156dc08eec94" class="cnblogs_code_hide">
<pre><span style="color: #008080;"> 1</span> State [<span style="color: #800080;">1</span><span style="color: #000000;">]:
</span><span style="color: #008080;"> 2</span> &lt;S&gt; ::= . <span style="color: #800000;">"</span><span style="color: #800000;">(</span><span style="color: #800000;">"</span> &lt;L&gt; <span style="color: #800000;">"</span><span style="color: #800000;">)</span><span style="color: #800000;">"</span> ;, <span style="color: #800000;">"</span><span style="color: #800000;">$</span><span style="color: #800000;">"</span>
<span style="color: #008080;"> 3</span> &lt;S<span style="color: #800000;">'</span><span style="color: #800000;">&gt; ::= . &lt;S&gt; "$" ;, "$"</span>
<span style="color: #008080;"> 4</span> &lt;S&gt; ::= . <span style="color: #800000;">"</span><span style="color: #800000;">x</span><span style="color: #800000;">"</span> ;, <span style="color: #800000;">"</span><span style="color: #800000;">$</span><span style="color: #800000;">"</span>
<span style="color: #008080;"> 5</span> State [<span style="color: #800080;">8</span><span style="color: #000000;">]:
</span><span style="color: #008080;"> 6</span> &lt;S&gt; ::= <span style="color: #800000;">"</span><span style="color: #800000;">(</span><span style="color: #800000;">"</span> &lt;L&gt; <span style="color: #800000;">"</span><span style="color: #800000;">)</span><span style="color: #800000;">"</span> . ;, <span style="color: #800000;">"</span><span style="color: #800000;">$</span><span style="color: #800000;">""</span><span style="color: #800000;">,</span><span style="color: #800000;">""</span><span style="color: #800000;">)</span><span style="color: #800000;">"</span>
<span style="color: #008080;"> 7</span> State [<span style="color: #800080;">4</span><span style="color: #000000;">]:
</span><span style="color: #008080;"> 8</span> &lt;S&gt; ::= <span style="color: #800000;">"</span><span style="color: #800000;">x</span><span style="color: #800000;">"</span> . ;, <span style="color: #800000;">"</span><span style="color: #800000;">$</span><span style="color: #800000;">""</span><span style="color: #800000;">,</span><span style="color: #800000;">""</span><span style="color: #800000;">)</span><span style="color: #800000;">"</span>
<span style="color: #008080;"> 9</span> State [<span style="color: #800080;">6</span><span style="color: #000000;">]:
</span><span style="color: #008080;">10</span> &lt;L&gt; ::= &lt;S&gt; . ;, <span style="color: #800000;">"</span><span style="color: #800000;">,</span><span style="color: #800000;">""</span><span style="color: #800000;">)</span><span style="color: #800000;">"</span>
<span style="color: #008080;">11</span> State [<span style="color: #800080;">9</span><span style="color: #000000;">]:
</span><span style="color: #008080;">12</span> &lt;L&gt; ::= &lt;L&gt; <span style="color: #800000;">"</span><span style="color: #800000;">,</span><span style="color: #800000;">"</span> &lt;S&gt; . ;, <span style="color: #800000;">"</span><span style="color: #800000;">,</span><span style="color: #800000;">""</span><span style="color: #800000;">)</span><span style="color: #800000;">"</span>
<span style="color: #008080;">13</span> State [<span style="color: #800080;">5</span><span style="color: #000000;">]:
</span><span style="color: #008080;">14</span> &lt;L&gt; ::= &lt;L&gt; . <span style="color: #800000;">"</span><span style="color: #800000;">,</span><span style="color: #800000;">"</span> &lt;S&gt; ;, <span style="color: #800000;">"</span><span style="color: #800000;">,</span><span style="color: #800000;">""</span><span style="color: #800000;">)</span><span style="color: #800000;">"</span>
<span style="color: #008080;">15</span> &lt;S&gt; ::= <span style="color: #800000;">"</span><span style="color: #800000;">(</span><span style="color: #800000;">"</span> &lt;L&gt; . <span style="color: #800000;">"</span><span style="color: #800000;">)</span><span style="color: #800000;">"</span> ;, <span style="color: #800000;">"</span><span style="color: #800000;">$</span><span style="color: #800000;">""</span><span style="color: #800000;">,</span><span style="color: #800000;">""</span><span style="color: #800000;">)</span><span style="color: #800000;">"</span>
<span style="color: #008080;">16</span> State [<span style="color: #800080;">7</span><span style="color: #000000;">]:
</span><span style="color: #008080;">17</span> &lt;S&gt; ::= . <span style="color: #800000;">"</span><span style="color: #800000;">(</span><span style="color: #800000;">"</span> &lt;L&gt; <span style="color: #800000;">"</span><span style="color: #800000;">)</span><span style="color: #800000;">"</span> ;, <span style="color: #800000;">"</span><span style="color: #800000;">,</span><span style="color: #800000;">""</span><span style="color: #800000;">)</span><span style="color: #800000;">"</span>
<span style="color: #008080;">18</span> &lt;S&gt; ::= . <span style="color: #800000;">"</span><span style="color: #800000;">x</span><span style="color: #800000;">"</span> ;, <span style="color: #800000;">"</span><span style="color: #800000;">,</span><span style="color: #800000;">""</span><span style="color: #800000;">)</span><span style="color: #800000;">"</span>
<span style="color: #008080;">19</span> &lt;L&gt; ::= &lt;L&gt; <span style="color: #800000;">"</span><span style="color: #800000;">,</span><span style="color: #800000;">"</span> . &lt;S&gt; ;, <span style="color: #800000;">"</span><span style="color: #800000;">,</span><span style="color: #800000;">""</span><span style="color: #800000;">)</span><span style="color: #800000;">"</span>
<span style="color: #008080;">20</span> State [<span style="color: #800080;">2</span><span style="color: #000000;">]:
</span><span style="color: #008080;">21</span> &lt;S&gt; ::= . <span style="color: #800000;">"</span><span style="color: #800000;">(</span><span style="color: #800000;">"</span> &lt;L&gt; <span style="color: #800000;">"</span><span style="color: #800000;">)</span><span style="color: #800000;">"</span> ;, <span style="color: #800000;">"</span><span style="color: #800000;">,</span><span style="color: #800000;">""</span><span style="color: #800000;">)</span><span style="color: #800000;">"</span>
<span style="color: #008080;">22</span> &lt;S&gt; ::= . <span style="color: #800000;">"</span><span style="color: #800000;">x</span><span style="color: #800000;">"</span> ;, <span style="color: #800000;">"</span><span style="color: #800000;">,</span><span style="color: #800000;">""</span><span style="color: #800000;">)</span><span style="color: #800000;">"</span>
<span style="color: #008080;">23</span> &lt;S&gt; ::= <span style="color: #800000;">"</span><span style="color: #800000;">(</span><span style="color: #800000;">"</span> . &lt;L&gt; <span style="color: #800000;">"</span><span style="color: #800000;">)</span><span style="color: #800000;">"</span> ;, <span style="color: #800000;">"</span><span style="color: #800000;">$</span><span style="color: #800000;">""</span><span style="color: #800000;">,</span><span style="color: #800000;">""</span><span style="color: #800000;">)</span><span style="color: #800000;">"</span>
<span style="color: #008080;">24</span> &lt;L&gt; ::= . &lt;L&gt; <span style="color: #800000;">"</span><span style="color: #800000;">,</span><span style="color: #800000;">"</span> &lt;S&gt; ;, <span style="color: #800000;">"</span><span style="color: #800000;">,</span><span style="color: #800000;">""</span><span style="color: #800000;">)</span><span style="color: #800000;">"</span>
<span style="color: #008080;">25</span> &lt;L&gt; ::= . &lt;S&gt; ;, <span style="color: #800000;">"</span><span style="color: #800000;">,</span><span style="color: #800000;">""</span><span style="color: #800000;">)</span><span style="color: #800000;">"</span>
<span style="color: #008080;">26</span> State [<span style="color: #800080;">3</span><span style="color: #000000;">]:
</span><span style="color: #008080;">27</span> &lt;S<span style="color: #800000;">'</span><span style="color: #800000;">&gt; ::= &lt;S&gt; . "$" ;, "$"</span></pre>
</div>
<span class="cnblogs_code_collapse">正确的</span></div>
<h2>CodeDom</h2>
<p>CodeDom不支持readonly属性，实在是遗憾。CodeDom还会对以"__"开头的变量自动添加个@前缀，真是无语。</p>
<div class="cnblogs_code">
<pre><span style="color: #008080;"> 1</span> <span style="color: #008000;">//</span><span style="color: #008000;"> private static TreeNodeType NODE__Grammar = new TreeNodeType(ContextfreeGrammarSLRTreeNodeType.NODE__Grammar, "Grammar", "&lt;Grammar&gt;");</span>
<span style="color: #008080;"> 2</span> CodeMemberField field = <span style="color: #0000ff;">new</span> CodeMemberField(<span style="color: #0000ff;">typeof</span><span style="color: #000000;">(TreeNodeType), GetNodeNameInParser(node));
</span><span style="color: #008080;"> 3</span> <span style="color: #008000;">//</span><span style="color: #008000;"> field.Attributes 不支持readonly，遗憾了。</span>
<span style="color: #008080;"> 4</span> field.Attributes = MemberAttributes.Private |<span style="color: #000000;"> MemberAttributes.Static;
</span><span style="color: #008080;"> 5</span> <span style="color: #0000ff;">var</span> ctor = <span style="color: #0000ff;">new</span> CodeObjectCreateExpression(<span style="color: #0000ff;">typeof</span><span style="color: #000000;">(TreeNodeType),
</span><span style="color: #008080;"> 6</span>     <span style="color: #0000ff;">new</span><span style="color: #000000;"> CodeFieldReferenceExpression(
</span><span style="color: #008080;"> 7</span>         <span style="color: #0000ff;">new</span><span style="color: #000000;"> CodeTypeReferenceExpression(GetTreeNodeConstTypeName(grammarId, algorithm)),
</span><span style="color: #008080;"> 8</span> <span style="color: #000000;">        GetNodeNameInParser(node)),
</span><span style="color: #008080;"> 9</span>     <span style="color: #0000ff;">new</span><span style="color: #000000;"> CodePrimitiveExpression(node.Content),
</span><span style="color: #008080;">10</span>     <span style="color: #0000ff;">new</span><span style="color: #000000;"> CodePrimitiveExpression(node.Nickname));
</span><span style="color: #008080;">11</span> field.InitExpression = ctor;</pre>
</div>
<h2>复杂的词法分析器</h2>
<p>从算法上说，理解语法分析器要比较理解词法分析器困难的多。但是LR语法分析器的结构却比词法分析器的结构和LL语法分析器的结果简单得多。目前实现<span style="color: #ff0000;">dump词法分析器代码</span>的代码是最绕的。要处理注释（//和/**/）是其中最复杂的问题。这段代码写好了我再也不想动了。</p>
<h2>LL和LR</h2>
<p>LR分析法确实比LL强太多。其适用各种现今的程序语言，对文法的限制极少，分析器结构还十分简单。奇妙的是，稍微改动下文法，就可以减少LR分析的state，精简代码。</p>
<p>例如ContextfreeGrammarCompiler的文法，稍微改改会有不同的state数目。</p>
<div class="cnblogs_code" onclick="cnblogs_code_show('6c6ff319-e9de-4acb-a047-bc654e02e0e2')"><img id="code_img_closed_6c6ff319-e9de-4acb-a047-bc654e02e0e2" class="code_img_closed" src="http://images.cnblogs.com/OutliningIndicators/ContractedBlock.gif" alt="" /><img id="code_img_opened_6c6ff319-e9de-4acb-a047-bc654e02e0e2" class="code_img_opened" style="display: none;" onclick="cnblogs_code_hide('6c6ff319-e9de-4acb-a047-bc654e02e0e2',event)" src="http://images.cnblogs.com/OutliningIndicators/ExpandedBlockStart.gif" alt="" />
<div id="cnblogs_code_open_6c6ff319-e9de-4acb-a047-bc654e02e0e2" class="cnblogs_code_hide">
<pre><span style="color: #008080;"> 1</span> ====================================================================
<span style="color: #008080;"> 2</span> <span style="color: #800080;">135</span> <span style="color: #0000ff;">set</span><span style="color: #000000;"> action items
</span><span style="color: #008080;"> 3</span> &lt;Grammar&gt; ::= &lt;ProductionList&gt; &lt;Production&gt;<span style="color: #000000;"> ;
</span><span style="color: #008080;"> 4</span> &lt;ProductionList&gt; ::= &lt;ProductionList&gt; &lt;Production&gt; | <span style="color: #0000ff;">null</span><span style="color: #000000;"> ;
</span><span style="color: #008080;"> 5</span> &lt;Production&gt; ::= &lt;Vn&gt; <span style="color: #800000;">"</span><span style="color: #800000;">::=</span><span style="color: #800000;">"</span> &lt;Canditate&gt; &lt;RightPartList&gt; <span style="color: #800000;">"</span><span style="color: #800000;">;</span><span style="color: #800000;">"</span><span style="color: #000000;"> ;
</span><span style="color: #008080;"> 6</span> &lt;Canditate&gt; ::= &lt;V&gt; &lt;VList&gt;<span style="color: #000000;"> ;
</span><span style="color: #008080;"> 7</span> &lt;VList&gt; ::= &lt;V&gt; &lt;VList&gt; | <span style="color: #0000ff;">null</span><span style="color: #000000;"> ;
</span><span style="color: #008080;"> 8</span> &lt;RightPartList&gt; ::= <span style="color: #800000;">"</span><span style="color: #800000;">|</span><span style="color: #800000;">"</span> &lt;Canditate&gt; &lt;RightPartList&gt; | <span style="color: #0000ff;">null</span><span style="color: #000000;"> ;
</span><span style="color: #008080;"> 9</span> &lt;V&gt; ::= &lt;Vn&gt; | &lt;Vt&gt;<span style="color: #000000;"> ;
</span><span style="color: #008080;">10</span> &lt;Vn&gt; ::= <span style="color: #800000;">"</span><span style="color: #800000;">&lt;</span><span style="color: #800000;">"</span> identifier <span style="color: #800000;">"</span><span style="color: #800000;">&gt;</span><span style="color: #800000;">"</span><span style="color: #000000;"> ;
</span><span style="color: #008080;">11</span> &lt;Vt&gt; ::= <span style="color: #800000;">"</span><span style="color: #800000;">null</span><span style="color: #800000;">"</span> | <span style="color: #800000;">"</span><span style="color: #800000;">identifier</span><span style="color: #800000;">"</span> | <span style="color: #800000;">"</span><span style="color: #800000;">number</span><span style="color: #800000;">"</span> | <span style="color: #800000;">"</span><span style="color: #800000;">constString</span><span style="color: #800000;">"</span> |<span style="color: #000000;"> constString ;
</span><span style="color: #008080;">12</span> ====================================================================
<span style="color: #008080;">13</span> <span style="color: #800080;">143</span> <span style="color: #0000ff;">set</span><span style="color: #000000;"> action items
</span><span style="color: #008080;">14</span> &lt;Grammar&gt; ::= &lt;Production&gt; &lt;ProductionList&gt;<span style="color: #000000;"> ;
</span><span style="color: #008080;">15</span> &lt;ProductionList&gt; ::= &lt;Production&gt; &lt;ProductionList&gt; | <span style="color: #0000ff;">null</span><span style="color: #000000;"> ;
</span><span style="color: #008080;">16</span> &lt;Production&gt; ::= &lt;Vn&gt; <span style="color: #800000;">"</span><span style="color: #800000;">::=</span><span style="color: #800000;">"</span> &lt;Canditate&gt; &lt;RightPartList&gt; <span style="color: #800000;">"</span><span style="color: #800000;">;</span><span style="color: #800000;">"</span><span style="color: #000000;"> ;
</span><span style="color: #008080;">17</span> &lt;Canditate&gt; ::= &lt;V&gt; &lt;VList&gt;<span style="color: #000000;"> ;
</span><span style="color: #008080;">18</span> &lt;VList&gt; ::= &lt;V&gt; &lt;VList&gt; | <span style="color: #0000ff;">null</span><span style="color: #000000;"> ;
</span><span style="color: #008080;">19</span> &lt;RightPartList&gt; ::= <span style="color: #800000;">"</span><span style="color: #800000;">|</span><span style="color: #800000;">"</span> &lt;Canditate&gt; &lt;RightPartList&gt; | <span style="color: #0000ff;">null</span><span style="color: #000000;"> ;
</span><span style="color: #008080;">20</span> &lt;V&gt; ::= &lt;Vn&gt; | &lt;Vt&gt;<span style="color: #000000;"> ;
</span><span style="color: #008080;">21</span> &lt;Vn&gt; ::= <span style="color: #800000;">"</span><span style="color: #800000;">&lt;</span><span style="color: #800000;">"</span> identifier <span style="color: #800000;">"</span><span style="color: #800000;">&gt;</span><span style="color: #800000;">"</span><span style="color: #000000;"> ;
</span><span style="color: #008080;">22</span> &lt;Vt&gt; ::= <span style="color: #800000;">"</span><span style="color: #800000;">null</span><span style="color: #800000;">"</span> | <span style="color: #800000;">"</span><span style="color: #800000;">identifier</span><span style="color: #800000;">"</span> | <span style="color: #800000;">"</span><span style="color: #800000;">number</span><span style="color: #800000;">"</span> | <span style="color: #800000;">"</span><span style="color: #800000;">constString</span><span style="color: #800000;">"</span> |<span style="color: #000000;"> constString ;
</span><span style="color: #008080;">23</span> ====================================================================
<span style="color: #008080;">24</span> <span style="color: #800080;">139</span> <span style="color: #0000ff;">set</span><span style="color: #000000;"> action items
</span><span style="color: #008080;">25</span> &lt;Grammar&gt; ::= &lt;ProductionList&gt; &lt;Production&gt;<span style="color: #000000;"> ;
</span><span style="color: #008080;">26</span> &lt;ProductionList&gt; ::= &lt;ProductionList&gt; &lt;Production&gt; | <span style="color: #0000ff;">null</span><span style="color: #000000;"> ;
</span><span style="color: #008080;">27</span> &lt;Production&gt; ::= &lt;Vn&gt; <span style="color: #800000;">"</span><span style="color: #800000;">::=</span><span style="color: #800000;">"</span> &lt;LeftPartList&gt; &lt;Canditate&gt; <span style="color: #800000;">"</span><span style="color: #800000;">;</span><span style="color: #800000;">"</span><span style="color: #000000;"> ;
</span><span style="color: #008080;">28</span> &lt;LeftPartList&gt; ::= &lt;LeftPartList&gt; &lt;LeftPart&gt; | <span style="color: #0000ff;">null</span><span style="color: #000000;"> ;
</span><span style="color: #008080;">29</span> &lt;LeftPart&gt; ::= &lt;Canditate&gt; <span style="color: #800000;">"</span><span style="color: #800000;">|</span><span style="color: #800000;">"</span><span style="color: #000000;"> ;
</span><span style="color: #008080;">30</span> &lt;Canditate&gt; ::= &lt;V&gt; &lt;VList&gt;<span style="color: #000000;"> ;
</span><span style="color: #008080;">31</span> &lt;VList&gt; ::= &lt;V&gt; &lt;VList&gt; | <span style="color: #0000ff;">null</span><span style="color: #000000;"> ;
</span><span style="color: #008080;">32</span> &lt;V&gt; ::= &lt;Vn&gt; | &lt;Vt&gt;<span style="color: #000000;"> ;
</span><span style="color: #008080;">33</span> &lt;Vn&gt; ::= <span style="color: #800000;">"</span><span style="color: #800000;">&lt;</span><span style="color: #800000;">"</span> identifier <span style="color: #800000;">"</span><span style="color: #800000;">&gt;</span><span style="color: #800000;">"</span><span style="color: #000000;"> ;
</span><span style="color: #008080;">34</span> &lt;Vt&gt; ::= <span style="color: #800000;">"</span><span style="color: #800000;">null</span><span style="color: #800000;">"</span> | <span style="color: #800000;">"</span><span style="color: #800000;">identifier</span><span style="color: #800000;">"</span> | <span style="color: #800000;">"</span><span style="color: #800000;">number</span><span style="color: #800000;">"</span> | <span style="color: #800000;">"</span><span style="color: #800000;">constString</span><span style="color: #800000;">"</span> |<span style="color: #000000;"> constString ;
</span><span style="color: #008080;">35</span> ====================================================================
<span style="color: #008080;">36</span> <span style="color: #800080;">120</span> <span style="color: #0000ff;">set</span><span style="color: #000000;"> action items
</span><span style="color: #008080;">37</span> &lt;Grammar&gt; ::= &lt;ProductionList&gt; &lt;Production&gt;<span style="color: #000000;"> ;
</span><span style="color: #008080;">38</span> &lt;ProductionList&gt; ::= &lt;ProductionList&gt; &lt;Production&gt; | <span style="color: #0000ff;">null</span><span style="color: #000000;"> ;
</span><span style="color: #008080;">39</span> &lt;Production&gt; ::= &lt;Vn&gt; <span style="color: #800000;">"</span><span style="color: #800000;">::=</span><span style="color: #800000;">"</span> &lt;Canditate&gt; &lt;RightPartList&gt; <span style="color: #800000;">"</span><span style="color: #800000;">;</span><span style="color: #800000;">"</span><span style="color: #000000;"> ;
</span><span style="color: #008080;">40</span> &lt;Canditate&gt; ::= &lt;VList&gt; &lt;V&gt;<span style="color: #000000;"> ;
</span><span style="color: #008080;">41</span> &lt;VList&gt; ::= &lt;VList&gt; &lt;V&gt; | <span style="color: #0000ff;">null</span><span style="color: #000000;"> ;
</span><span style="color: #008080;">42</span> &lt;RightPartList&gt; ::= <span style="color: #800000;">"</span><span style="color: #800000;">|</span><span style="color: #800000;">"</span> &lt;Canditate&gt; &lt;RightPartList&gt; | <span style="color: #0000ff;">null</span><span style="color: #000000;"> ;
</span><span style="color: #008080;">43</span> &lt;V&gt; ::= &lt;Vn&gt; | &lt;Vt&gt;<span style="color: #000000;"> ;
</span><span style="color: #008080;">44</span> &lt;Vn&gt; ::= <span style="color: #800000;">"</span><span style="color: #800000;">&lt;</span><span style="color: #800000;">"</span> identifier <span style="color: #800000;">"</span><span style="color: #800000;">&gt;</span><span style="color: #800000;">"</span><span style="color: #000000;"> ;
</span><span style="color: #008080;">45</span> &lt;Vt&gt; ::= <span style="color: #800000;">"</span><span style="color: #800000;">null</span><span style="color: #800000;">"</span> | <span style="color: #800000;">"</span><span style="color: #800000;">identifier</span><span style="color: #800000;">"</span> | <span style="color: #800000;">"</span><span style="color: #800000;">number</span><span style="color: #800000;">"</span> | <span style="color: #800000;">"</span><span style="color: #800000;">constString</span><span style="color: #800000;">"</span> | constString ;</pre>
</div>
<span class="cnblogs_code_collapse">ContextfreeGrammars</span></div>
<h1>总结</h1>
<p>实现了LALR(1)分析和GLSL解析器。</p>
<p>今后做任何语言、格式的解析都不用愁了。</p>
