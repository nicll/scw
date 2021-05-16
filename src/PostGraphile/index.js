const express = require("express");
const { postgraphile } = require("postgraphile");

const app = express();

app.use(
    postgraphile(
        "postgres://" + process.env.SCW1_DBUSER_DYN + ":" + process.env.SCW1_DBPASS_DYN + "@localhost:5432/scw",
        "scw1_dyn",
        {
            watchPg: true,
            graphiql: true,
            enhanceGraphiql: true,
            subscriptions: true,
            ownerConnectionString: "postgres://" + process.env.SCW1_DBUSER_SYS + ":" + process.env.SCW1_DBPASS_SYS + "@localhost:5432/scw"
        }
    )
);

app.listen(4000);
