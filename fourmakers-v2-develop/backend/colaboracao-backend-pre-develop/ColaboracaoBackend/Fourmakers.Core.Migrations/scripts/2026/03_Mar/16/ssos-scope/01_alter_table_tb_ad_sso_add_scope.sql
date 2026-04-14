-- ------------------------------------------------------------------------------
-- Microsoft SSO / Teams: escopo OAuth alinhado ao app Flutter
-- Data: 2026-03-16
-- Adiciona coluna scope em tb_ad_sso para configurar os escopos na troca de code/refresh
-- (openid, profile, offline_access, User.Read, Calendars.ReadWrite, OnlineMeetings.ReadWrite).
-- Quando NULL, o backend usa "openid" (comportamento anterior).
-- ------------------------------------------------------------------------------

ALTER TABLE tb_ad_sso
    ADD COLUMN scope VARCHAR(500) NULL
    AFTER redirect_url;

UPDATE tb_ad_sso
SET scope = 'openid profile offline_access https://graph.microsoft.com/User.Read https://graph.microsoft.com/Calendars.ReadWrite https://graph.microsoft.com/OnlineMeetings.ReadWrite'
WHERE tb_org_id = 2;