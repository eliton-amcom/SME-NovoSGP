IF OBJECT_ID('tempdb..#tmpTurmasEja') IS NOT NULL
	DROP TABLE #tmpTurmasEja
CREATE TABLE #tmpTurmasEja 
(
	CodigoTurma int,
	SerieEnsino varchar(40),
	EtapaEja varchar(40),
	Processado BIT
)
INSERT INTO #tmpTurmasEja
select te.cd_turma_escola, serie_ensino.dc_serie_ensino, null, 0 from turma_escola te
left join serie_turma_escola ON serie_turma_escola.cd_turma_escola = te.cd_turma_escola
                             left join serie_turma_grade ON serie_turma_grade.cd_turma_escola = serie_turma_escola.cd_turma_escola
                             left join escola_grade ON serie_turma_grade.cd_escola_grade = escola_grade.cd_escola_grade
                             left join grade ON escola_grade.cd_grade = grade.cd_grade
                             left join serie_ensino
                                       ON grade.cd_serie_ensino = serie_ensino.cd_serie_ensino
where te.cd_turma_escola in (
'2111770',
'2111782',
'2111784',
'2111786',
'2111789',
'2114851',
'2114854',
'2114857',
'2114860',
'2114862',
'2114864',
'2114868',
'2114869',
'2115790',
'2115791',
'2115792',
'2115793',
'2115794',
'2115795',
'2115796',
'2115797',
'2115799',
'2115801',
'2115802',
'2115803',
'2117091',
'2117097',
'2117105',
'2117112',
'2117120',
'2117123',
'2117128',
'2117141',
'2117911',
'2117916',
'2117919',
'2117927',
'2117930',
'2117934',
'2117943',
'2117947',
'2117951',
'2117956',
'2118036',
'2118038',
'2118039',
'2118041',
'2118042',
'2118045',
'2118048',
'2118051',
'2118053',
'2118055',
'2118155',
'2118158',
'2118159',
'2118161',
'2118162',
'2118164',
'2118165',
'2118166',
'2118167',
'2118168',
'2118321',
'2119614',
'2119618',
'2119627',
'2119635',
'2119674',
'2119695',
'2119697',
'2119700',
'2119702',
'2119704',
'2119860',
'2120401',
'2120405',
'2120410',
'2120413',
'2120417',
'2120418',
'2120420',
'2120422',
'2120425',
'2122776',
'2122781',
'2122791',
'2122800',
'2122804',
'2122810',
'2122819',
'2122842',
'2122862',
'2123853',
'2123855',
'2123857',
'2123859',
'2123861',
'2123864',
'2123866',
'2123867',
'2123868',
'2123869',
'2124650',
'2124651',
'2124652',
'2124653',
'2124654',
'2124655',
'2124656',
'2124657',
'2124658',
'2124659',
'2126749',
'2126752',
'2126753',
'2126754',
'2126755',
'2126761',
'2126764',
'2126767',
'2126769',
'2126911',
'2126923',
'2126928',
'2126933',
'2126936',
'2129062',
'2129063',
'2129065',
'2129066',
'2129069',
'2129071',
'2129072',
'2129074',
'2129075',
'2129076',
'2129078',
'2129080',
'2129274',
'2129381',
'2130137',
'2130143',
'2130412',
'2130417',
'2131873',
'2131880',
'2131888',
'2131893',
'2135837',
'2135839',
'2135840',
'2135842',
'2135845',
'2135847',
'2135848',
'2135849',
'2135851',
'2135852',
'2135854',
'2135857',
'2136600',
'2136626',
'2136641',
'2136659',
'2136671',
'2136685',
'2136700',
'2136725',
'2136735',
'2136744',
'2136754',
'2136791',
'2157128',
'2157133',
'2157136',
'2157139',
'2157142',
'2157149',
'2157156',
'2157161',
'2157165',
'2157169',
'2157184',
'2157185',
'2157186',
'2157187',
'2157188',
'2157189',
'2157190',
'2157191',
'2157192',
'2157193',
'2190785',
'2197437',
'2214235',
'2219999',
'2103109',
'2110372',
'2110374',
'2110376',
'2110770',
'2110781',
'2111484',
'2111503',
'2111544',
'2114541',
'2114546',
'2116460',
'2117069',
'2117148',
'2117156',
'2117537',
'2117653',
'2117981',
'2117986',
'2117989',
'2117993',
'2117998',
'2118002',
'2118071',
'2118080',
'2118086',
'2119088',
'2119097',
'2119553',
'2119741',
'2119751',
'2120389',
'2120398',
'2120636',
'2120639',
'2120641',
'2120643',
'2120957',
'2121615',
'2121638',
'2121649',
'2121673',
'2121679',
'2122285',
'2122303',
'2122392',
'2122399',
'2122624',
'2122658',
'2122676',
'2122686',
'2122725',
'2122774',
'2122801',
'2122871',
'2122907',
'2122939',
'2122962',
'2123131',
'2123145',
'2123377',
'2124717',
'2124732',
'2124745',
'2124774',
'2124961',
'2124982',
'2124994',
'2126383',
'2126463',
'2126657',
'2126660',
'2126661',
'2126662',
'2126667',
'2126710',
'2126751',
'2126770',
'2126773',
'2126785',
'2126788',
'2126818',
'2126914',
'2126931',
'2126988',
'2126996',
'2127032',
'2127040',
'2127051',
'2127059',
'2127066',
'2127075',
'2127080',
'2127442',
'2128157',
'2128456',
'2128536',
'2128549',
'2128555',
'2129116',
'2129117',
'2129120',
'2129121',
'2129124',
'2129135',
'2129137',
'2129141',
'2129152',
'2129158',
'2129159',
'2129162',
'2129164',
'2129418',
'2129426',
'2129604',
'2129826',
'2130182',
'2130258',
'2130272',
'2130277',
'2130280',
'2130411',
'2130498',
'2130514',
'2130527',
'2130988',
'2131010',
'2131013',
'2131216',
'2131230',
'2131240',
'2131522',
'2131525',
'2131562',
'2131571',
'2131573',
'2131588',
'2131590',
'2131596',
'2131599',
'2132224',
'2132235',
'2132309',
'2132364',
'2132384',
'2132415',
'2133143',
'2133168',
'2133191',
'2133202',
'2133603',
'2133688',
'2133732',
'2133829',
'2133830',
'2133934',
'2133950',
'2133979',
'2134176',
'2134190',
'2134202',
'2134699',
'2134734',
'2134810',
'2134818',
'2134854',
'2134856',
'2134869',
'2134870',
'2134889',
'2134894',
'2134899',
'2134903',
'2134905',
'2134914',
'2134924',
'2134936',
'2135046',
'2135099',
'2135151',
'2135410',
'2135554',
'2135572',
'2135602',
'2135618',
'2135623',
'2135629',
'2135633',
'2135718',
'2135719',
'2135721',
'2135724',
'2135732',
'2135745',
'2135750',
'2135757',
'2135811',
'2135886',
'2135888',
'2135890',
'2135913',
'2135916',
'2135920',
'2136061',
'2136076',
'2136094',
'2136100',
'2136141',
'2136190',
'2136195',
'2136201',
'2136273',
'2136299',
'2136332',
'2136463',
'2136560',
'2136577',
'2136583',
'2136798',
'2136810',
'2136821',
'2137039',
'2137097',
'2137112',
'2137203',
'2137233',
'2137246',
'2137258',
'2137759',
'2137794',
'2137809',
'2137822',
'2137843',
'2137846',
'2137849',
'2137856',
'2137936',
'2137987',
'2138075',
'2138171',
'2138174',
'2138245',
'2138306',
'2138421',
'2138442',
'2138451',
'2138467',
'2138597',
'2138655',
'2138673',
'2138698',
'2139072',
'2139079',
'2139091',
'2139098',
'2139172',
'2139211',
'2139218',
'2139239',
'2139351',
'2139357',
'2139369',
'2139371',
'2139401',
'2139434',
'2139470',
'2139481',
'2139493',
'2139600',
'2139607',
'2139637',
'2139665',
'2139672',
'2139768',
'2139773',
'2139850',
'2139853',
'2139859',
'2139861',
'2139873',
'2139900',
'2139903',
'2140028',
'2140554',
'2140555',
'2140556',
'2140557',
'2140629',
'2140729',
'2142869',
'2142883',
'2142895',
'2142907',
'2142913',
'2142921',
'2142925',
'2142927',
'2142935',
'2142939',
'2143000',
'2143001',
'2143431',
'2152423',
'2152445',
'2155457',
'2155486',
'2155540',
'2155545',
'2157084',
'2157095',
'2157116',
'2157860',
'2157864',
'2157875',
'2158684',
'2158688',
'2158691',
'2159151',
'2159173',
'2159209',
'2167956',
'2167958',
'2170580',
'2170582',
'2173278',
'2173280',
'2177808',
'2177811',
'2178021',
'2178637',
'2179806',
'2181259',
'2181296',
'2187384',
'2189628',
'2190051',
'2190572',
'2190643',
'2190650',
'2201535',
'2207931',
'2207935',
'2208297',
'2210076',
'2210125',
'2210129',
'2210131',
'2210884',
'2211717',
'2211782',
'2213604',
'2214238',
'2215126',
'2216789',
'2216971',
'2218577',
'2218744',
'2219426',
'2219439',
'2219650',
'2220319',
'2220369',
'2220866',
'2221416',
'2221479',
'2221596',
'2221639',
'2222162',
'2222397',
'2222691',
'2222914',
'2225414',
'2225497',
'2258109',
'2258111',
'2258113',
'2110373',
'2110375',
'2110377',
'2110773',
'2110778',
'2111483',
'2111502',
'2111508',
'2111545',
'2114536',
'2114548',
'2116457',
'2116462',
'2117049',
'2117085',
'2117110',
'2117161',
'2117170',
'2117180',
'2117511',
'2117556',
'2117771',
'2117786',
'2117892',
'2117903',
'2117972',
'2117976',
'2117995',
'2118006',
'2118009',
'2118075',
'2118077',
'2118087',
'2118088',
'2119536',
'2119570',
'2119736',
'2119747',
'2119754',
'2120399',
'2120617',
'2120635',
'2120637',
'2120640',
'2120642',
'2120644',
'2120645',
'2120921',
'2120967',
'2121606',
'2121624',
'2121657',
'2121664',
'2121684',
'2122293',
'2122311',
'2122412',
'2122423',
'2122648',
'2122651',
'2122665',
'2122684',
'2122749',
'2122761',
'2122772',
'2122787',
'2122811',
'2122900',
'2122934',
'2122949',
'2122958',
'2122972',
'2123137',
'2123153',
'2123374',
'2123393',
'2123419',
'2123790',
'2123846',
'2124723',
'2124739',
'2124768',
'2124953',
'2124970',
'2124989',
'2125000',
'2126397',
'2126412',
'2126656',
'2126658',
'2126659',
'2126663',
'2126664',
'2126665',
'2126706',
'2126747',
'2126757',
'2126760',
'2126776',
'2126778',
'2126782',
'2126784',
'2126787',
'2126790',
'2126820',
'2126915',
'2126916',
'2126917',
'2126921',
'2126957',
'2126979',
'2126982',
'2126994',
'2127001',
'2127003',
'2127035',
'2127045',
'2127057',
'2127060',
'2127062',
'2127070',
'2127084',
'2127087',
'2127430',
'2127436',
'2127445',
'2127446',
'2128198',
'2128448',
'2128460',
'2128533',
'2128543',
'2128557',
'2128558',
'2129118',
'2129119',
'2129122',
'2129126',
'2129127',
'2129139',
'2129142',
'2129144',
'2129150',
'2129153',
'2129155',
'2129157',
'2129160',
'2129161',
'2129165',
'2129167',
'2129414',
'2129421',
'2129608',
'2129828',
'2129829',
'2130190',
'2130298',
'2130418',
'2130507',
'2130520',
'2130536',
'2130927',
'2130967',
'2130981',
'2130994',
'2131015',
'2131030',
'2131228',
'2131233',
'2131279',
'2131523',
'2131535',
'2131567',
'2131575',
'2131577',
'2131594',
'2131601',
'2131603',
'2131605',
'2131606',
'2131607',
'2131647',
'2132246',
'2132267',
'2132295',
'2132373',
'2132391',
'2132517',
'2133159',
'2133181',
'2133211',
'2133227',
'2133646',
'2133678',
'2133701',
'2133750',
'2133796',
'2133844',
'2133848',
'2133962',
'2134000',
'2134189',
'2134191',
'2134200',
'2134225',
'2134248',
'2134268',
'2134302',
'2134549',
'2134557',
'2134725',
'2134827',
'2134835',
'2134857',
'2134878',
'2134897',
'2134900',
'2134908',
'2134913',
'2134916',
'2134937',
'2134943',
'2134951',
'2134955',
'2134957',
'2135030',
'2135061',
'2135106',
'2135147',
'2135167',
'2135441',
'2135467',
'2135532',
'2135564',
'2135581',
'2135625',
'2135636',
'2135662',
'2135720',
'2135722',
'2135726',
'2135728',
'2135730',
'2135734',
'2135748',
'2135752',
'2135754',
'2135814',
'2135887',
'2135889',
'2135891',
'2135892',
'2135912',
'2135914',
'2135915',
'2135922',
'2135925',
'2136071',
'2136101',
'2136113',
'2136128',
'2136145',
'2136179',
'2136193',
'2136204',
'2136210',
'2136258',
'2136265',
'2136277',
'2136317',
'2136349',
'2136360',
'2136494',
'2136510',
'2136543',
'2136579',
'2136589',
'2136783',
'2136851',
'2136883',
'2137004',
'2137058',
'2137077',
'2137095',
'2137104',
'2137116',
'2137122',
'2137210',
'2137241',
'2137248',
'2137263',
'2137269',
'2137790',
'2137829',
'2137854',
'2137861',
'2137871',
'2137882',
'2137887',
'2137913',
'2137948',
'2138158',
'2138181',
'2138194',
'2138293',
'2138344',
'2138379',
'2138410',
'2138414',
'2138433',
'2138460',
'2138479',
'2138580',
'2138604',
'2138633',
'2138665',
'2138684',
'2138731',
'2138761',
'2139171',
'2139200',
'2139226',
'2139236',
'2139271',
'2139336',
'2139363',
'2139383',
'2139396',
'2139402',
'2139405',
'2139410',
'2139411',
'2139460',
'2139472',
'2139505',
'2139513',
'2139517',
'2139520',
'2139526',
'2139611',
'2139648',
'2139658',
'2139684',
'2139690',
'2139697',
'2139709',
'2139743',
'2139779',
'2139784',
'2139785',
'2139847',
'2139854',
'2139855',
'2139862',
'2139865',
'2139876',
'2139901',
'2139905',
'2139931',
'2139933',
'2139938',
'2140026',
'2140031',
'2140170',
'2140558',
'2140645',
'2140730',
'2142862',
'2142872',
'2142881',
'2142886',
'2142898',
'2142901',
'2142909',
'2142915',
'2142923',
'2142929',
'2142931',
'2142943',
'2142947',
'2143002',
'2143003',
'2143432',
'2144834',
'2152430',
'2152447',
'2155459',
'2155491',
'2155550',
'2155640',
'2157076',
'2157091',
'2157100',
'2157121',
'2157127',
'2157862',
'2157865',
'2157877',
'2158682',
'2158685',
'2158686',
'2158758',
'2159163',
'2159201',
'2159225',
'2167957',
'2167959',
'2173279',
'2173281',
'2176114',
'2176624',
'2177807',
'2177967',
'2179754',
'2179954',
'2180282',
'2181287',
'2181291',
'2181775',
'2182068',
'2182874',
'2184877',
'2188069',
'2188814',
'2189641',
'2190058',
'2190132',
'2190488',
'2191003',
'2204994',
'2205039',
'2207656',
'2209769',
'2209778',
'2210123',
'2210130',
'2210132',
'2210998',
'2211513',
'2211747',
'2212000',
'2212230',
'2213017',
'2214240',
'2217292',
'2218730',
'2219628',
'2220367',
'2220429',
'2220671',
'2221418',
'2221813',
'2222401',
'2222480',
'2222547',
'2258110',
'2258112',
'2258114'
)

Declare @CodigoTurma int
Declare @SerieEnsino varchar(40)

Declare @Etapa varchar(5)
Declare @EtapaFinal varchar(1) = 0

Declare @TamanhoSerieEnsino int
Declare @IndexPrimeiroCiclo int
Declare @IndexSegundoCiclo int

While (Select Count(*) From #tmpTurmasEja Where Processado = 0) > 0
Begin
    Select Top 1 @CodigoTurma = CodigoTurma, @SerieEnsino = SerieEnsino  From #tmpTurmasEja Where Processado = 0

	Set @TamanhoSerieEnsino = (select len(@SerieEnsino))
	Set @Etapa = (SELECT RTRIM(LTRIM(SUBSTRING(@SerieEnsino, @TamanhoSerieEnsino - 1, @TamanhoSerieEnsino))));

	PRINT @CodigoTurma
	PRINT @SerieEnsino
	PRINT @TamanhoSerieEnsino
	PRINT @Etapa

    SET @IndexPrimeiroCiclo = (SELECT CHARINDEX(' I ', @Etapa)) 
    SET @IndexSegundoCiclo = (SELECT CHARINDEX(' II', @SerieEnsino)) 

	PRINT @IndexPrimeiroCiclo
	PRINT @IndexSegundoCiclo

    if ((@Etapa = 'I' and @IndexSegundoCiclo <= 0) OR (@IndexPrimeiroCiclo > 0 and @IndexSegundoCiclo <= 0))
        SET @EtapaFinal = 1;
    else if ((@Etapa = 'II' AND @IndexPrimeiroCiclo <= 0) OR (@IndexPrimeiroCiclo <= 0 AND @IndexSegundoCiclo > 0))
        SET @EtapaFinal = 2;

	PRINT @EtapaFinal

    Update #tmpTurmasEja Set EtapaEja = @EtapaFinal, Processado = 1 Where CodigoTurma = @CodigoTurma

End

SELECT * FROM #tmpTurmasEja group by CodigoTurma, SerieEnsino, EtapaEja, Processado order by CodigoTurma, EtapaEja