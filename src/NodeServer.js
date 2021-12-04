const express = require("express");
const { postgraphile } = require("postgraphile");
const angapp = express();
const { readFileSync , writeFileSync, readdirSync} = require('node:fs');

var files = readdirSync('public',{encoding:"ascii"}); //Gets all files in public Directory for finding the correct main.js file

var mainfile;

files.forEach(file => { 
	if(file.match("main\..+\.js"))
		mainfile=file;
})

console.log(mainfile);

var content = readFileSync('public/'+mainfile,{encoding:"ascii"});

if(content.includes('"SCW1_GRAPHQLURI"'))
{
	content = content.replace('SCW1_GRAPHQLURI', process.env.SCW1_GRAPHQLURI)
	console.log("replaced first string")
}
if(content.includes('SCW1_ASPURI'))
{
	content = content.replace('SCW1_ASPURI', process.env.SCW1_ASPURI)
	console.log("replaced second string")
}

writeFileSync('public/'+mainfile, content)

angapp.use(express.static('public'));

angapp.listen(80);
console.log("angular app served");

const graphapp = express();

graphapp.use(
    postgraphile(
        "postgres://" + process.env.SCW1_DB_USER_DYN + ":" + process.env.SCW1_DB_PASS_DYN
            + "@" + process.env.SCW1_DB_HOST + ":" + process.env.SCW1_DB_PORT + "/scw",
        "scw1_dyn",
        {
            watchPg: true,
            graphiql: true,
            enhanceGraphiql: true,
            subscriptions: true,
            enableCors: true,
			showErrorStack: true,
			extendedErrors: ['hint', 'detail', 'errcode'],
            graphqlRoute: process.env.SCW1_PGRAPH_PATH,
            ownerConnectionString: "postgres://" + process.env.SCW1_DB_USER_SYS + ":" + process.env.SCW1_DB_PASS_SYS
                + "@" + process.env.SCW1_DB_HOST + ":" + process.env.SCW1_DB_PORT + "/scw"
        }
    )
);

graphapp.listen(process.env.SCW1_PGRAPH_PORT);

console.log("grahphql app served");


console.log(process.env.SCW1_DB_USER_DYN)
console.log(process.env.SCW1_DB_PASS_DYN)
console.log(process.env.SCW1_DB_HOST)
console.log(process.env.SCW1_DB_PORT)
console.log(process.env.SCW1_PGRAPH_PATH)
console.log(process.env.SCW1_DB_USER_SYS)
console.log(process.env.SCW1_DB_PASS_SYS)
console.log(process.env.SCW1_PGRAPH_PORT)