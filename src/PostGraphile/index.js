const express = require("express");
const { postgraphile } = require("postgraphile");

const app = express();

app.use(
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
            graphqlRoute: process.env.SCW1_PGRAPHILE_ROUTE,
            ownerConnectionString: "postgres://" + process.env.SCW1_DB_USER_SYS + ":" + process.env.SCW1_DB_PASS_SYS
                + "@" + process.env.SCW1_DB_HOST + ":" + process.env.SCW1_DB_PORT + "/scw"
        }
    )
);

app.listen(process.env.SCW1_PGRAPHILE_PORT);
