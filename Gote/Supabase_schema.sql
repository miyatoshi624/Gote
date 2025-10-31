-- カテゴリテーブル
create table categories (
	id serial primary key,
	name varchar(100) not null,
	description text
);

-- メモテーブル
create table memos (
	id serial primary key,
	category_id integer references categories(id) on delete restrict,
	title varchar(200) not null,
	content text,
	created_at timestamp default current_timestamp,
	updated_at timestamp default current_timestamp
);

-- 検索用インテックス
create index idx_memos_title_content on memos using gin (to_tsvector('japanese', title || ' ' || content));