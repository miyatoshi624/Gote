-- カテゴリテーブル
create table categories (
	category_id uuid default uuid_generate_v4() primary key,
	user_id uuid not null,
	name text not null,
	description text
);

alter table categories enable row level security;

create policy "Allow select for owner" on categories
for select using (auth.uid() = user_id);

create policy "Allow insert for owner" on categories
for insert with check (auth.uid() = user_id);

create policy "Allow update for owner" on categories
for update using (auth.uid() = user_id);

create policy "Allow delete for owner" on categories
for delete using (auth.uid() = user_id);


-- メモテーブル
create table memos (
	memo_id uuid default uuid_generate_v4() primary key,
	user_id uuid not null,
	category_id uuid not null references categories(category_id) on delete restrict,
	title text not null,
	content text,
	created_at timestamp default current_timestamp,
	updated_at timestamp default current_timestamp
);

alter table memos enable row level security;

create policy "Allow select for owner" on memos
for select using (auth.uid() = user_id);

create policy "Allow insert for owner" on memos
for insert with check (auth.uid() = user_id);

create policy "Allow update for owner" on memos
for update using (auth.uid() = user_id);

create policy "Allow delete for owner" on memos
for delete using (auth.uid() = user_id);


-- 検索用インテックス
--create index idx_memos_title_content on memos using gin (to_tsvector('japanese', title || ' ' || content));