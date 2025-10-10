import { redirect } from "next/navigation";

const IndexPage = async () => {
	redirect("/user");
};

export default IndexPage;
