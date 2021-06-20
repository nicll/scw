const express = require("express");
const { postgraphile } = require("postgraphile");

const app = express();

app.use(
    postgraphile(
        "postgres://" + process.env.SCW1_DBUSER_DYN + ":" + process.env.SCW1_DBPASS_DYN
            + "@" + process.env.SCW1_DBSERVER + ":" + process.env.SCW1_DBPORT + "/scw",
        "scw1_dyn",
        {
            watchPg: true,
            graphiql: true,
            enhanceGraphiql: true,
            subscriptions: true,
            enableCors: true,
            ownerConnectionString: "postgres://" + process.env.SCW1_DBUSER_SYS + ":" + process.env.SCW1_DBPASS_SYS
                + "@" + process.env.SCW1_DBSERVER + ":" + process.env.SCW1_DBPORT + "/scw"
        }
    )
);

app.listen(4000);
