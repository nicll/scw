CREATE SCHEMA scw1_dyn
    AUTHORIZATION scw1_user_sys;

GRANT ALL ON SCHEMA scw1_dyn TO scw1_user_dyn;

GRANT ALL ON SCHEMA scw1_dyn TO scw1_user_sys;


-- sollte automatisch erstellt werden
-- nur verwenden, falls das nicht passiert
CREATE SCHEMA scw1_sys
    AUTHORIZATION scw1_user_sys;

