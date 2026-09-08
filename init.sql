CREATE TABLE IF NOT EXISTS players (
    player_id SERIAL PRIMARY KEY,
    username VARCHAR(100),
    email VARCHAR(100),
    password VARCHAR(100)
);

CREATE TABLE IF NOT EXISTS robots (
    robot_id SERIAL PRIMARY KEY,
    name VARCHAR(100)
);

CREATE TABLE IF NOT EXISTS players_robots (
    id SERIAL PRIMARY KEY,
    player_id integer REFERENCES players,
    robot_id integer REFERENCES robots
);

CREATE TABLE IF NOT EXISTS leaderboard (
    leaderboard_id  SERIAL PRIMARY KEY,
    player_id integer REFERENCES players,
    rank integer
);

CREATE TABLE IF NOT EXISTS gamesessions (
    id SERIAL PRIMARY KEY,
    host_player_id integer REFERENCES players,
    client_player_id integer REFERENCES players,
    host_robot_id integer REFERENCES robots,
    client_robot_id integer REFERENCES robots,
    start_time TIMESTAMP,
    end_time TIMESTAMP
);

INSERT INTO players (username, email, password) VALUES
    ('Tk', 'tk@test.com', 'toto'),
    ('Julian', 'julian@test.com', 'eee'),
    ('Quentin', 'quentin@test.com', '1234'),
    ('Mathilde', 'mathilde@test.com', 'password')
ON CONFLICT DO NOTHING;

INSERT INTO robots (name) VALUES
    ('PumaBot'),
    ('RoninBot'),
    ('SamuraiBot'),
    ('KnightBot')
ON CONFLICT DO NOTHING;

INSERT INTO players_robots (player_id, robot_id) VALUES
    (2, 1),
    (2, 2),
    (2, 3),
    (3, 1),
    (1, 1),
    (1, 2),
    (1, 3),
    (4, 2)
ON CONFLICT DO NOTHING;

INSERT INTO leaderboard (player_id, rank) VALUES
    (2, 1),
    (1, 2),
    (4, 3),
    (3, 4)
ON CONFLICT DO NOTHING;
